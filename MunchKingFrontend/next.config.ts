/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  eslint: {
    ignoreDuringBuilds: true, 
  },
  async rewrites() {
  return [
    {
      source: "/Auth/:path*",
      destination: "http://munchking-backend:5136/Auth/:path*",
    },
    {
      source: "/api/:path*",
      destination: "http://munchking-backend:5136/api/:path*",
    },
  ];
}
};

export default nextConfig;
