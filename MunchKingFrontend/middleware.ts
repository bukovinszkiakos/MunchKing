import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  if (pathname === "/auth/login" || pathname === "/auth/register") {
    return NextResponse.next();
  }

  const token = request.cookies.get("token")?.value;

  if (!token) {
    const loginUrl = request.nextUrl.clone();
    loginUrl.pathname = "/auth/login";
    return NextResponse.redirect(loginUrl);
  }

  try {
    const payloadBase64 = token.split(".")[1];
    const payloadJson = Buffer.from(payloadBase64, "base64").toString("utf-8");
    const payload = JSON.parse(payloadJson);

    if (Date.now() >= payload.exp * 1000) {
      const loginUrl = request.nextUrl.clone();
      loginUrl.pathname = "/auth/login";
      return NextResponse.redirect(loginUrl);
    }

    const roles = payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
    const isAdmin = Array.isArray(roles)
      ? roles.includes("Admin") || roles.includes("SuperAdmin")
      : roles === "Admin" || roles === "SuperAdmin";

    if (isAdmin && (pathname === "/" || pathname === "/menu" || pathname === "/profile")) {
      const adminUrl = request.nextUrl.clone();
      adminUrl.pathname = "/admin";
      return NextResponse.redirect(adminUrl);
    }

    if (!isAdmin && pathname.startsWith("/admin")) {
      const userUrl = request.nextUrl.clone();
      userUrl.pathname = "/";
      return NextResponse.redirect(userUrl);
    }

  } catch (err) {
    const loginUrl = request.nextUrl.clone();
    loginUrl.pathname = "/auth/login";
    return NextResponse.redirect(loginUrl);
  }

  return NextResponse.next();
}

export const config = {
  matcher: [
    "/menu",
    "/profile",
    "/cart",
    "/orders",
    "/admin/:path*",
  ],
};
