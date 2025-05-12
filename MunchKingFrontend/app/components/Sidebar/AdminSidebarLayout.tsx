"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { FaTachometerAlt, FaList, FaHamburger, FaUsers, FaClipboardList, FaComment } from "react-icons/fa";

const links = [
  { href: "/admin", label: "Dashboard", icon: <FaTachometerAlt /> },
  { href: "/admin/categories", label: "Categories", icon: <FaList /> },
  { href: "/admin/products", label: "Products", icon: <FaHamburger /> },
  { href: "/admin/orders", label: "Orders", icon: <FaClipboardList /> },
  { href: "/admin/users", label: "Users", icon: <FaUsers /> },
  { href: "/admin/contact", label: "Feedback", icon: <FaComment /> },
];

export default function AdminSidebarLayout({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();

  return (
    <div className="flex min-h-screen">
      <aside className="w-64 bg-[#1a1a1a] text-white flex flex-col py-6 px-4 shadow-md">
        <h2 className="text-2xl font-bold text-yellow-400 mb-8 font-serif">MunchKing Admin</h2>
        <nav className="flex flex-col gap-4">
          {links.map(({ href, label, icon }) => (
            <Link
              key={href}
              href={href}
              className={`flex items-center gap-3 px-3 py-2 rounded hover:bg-yellow-500/10 transition ${
                pathname === href ? "bg-yellow-500 text-black font-semibold" : "text-white"
              }`}
            >
              <span className="text-lg">{icon}</span>
              <span>{label}</span>
            </Link>
          ))}
        </nav>
      </aside>

      <main className="flex-1 bg-gray-100 p-6 overflow-auto">{children}</main>
    </div>
  );
}
