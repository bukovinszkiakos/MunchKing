"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useAuth } from "../../context/AuthContext";
import { useState } from "react";
import {
  FaTachometerAlt,
  FaList,
  FaHamburger,
  FaUsers,
  FaClipboardList,
  FaComment,
  FaUserShield,
  FaBars,
  FaTimes,
} from "react-icons/fa";

const baseLinks = [
  { href: "/admin", label: "Dashboard", icon: <FaTachometerAlt /> },
  { href: "/admin/categories", label: "Categories", icon: <FaList /> },
  { href: "/admin/products", label: "Products", icon: <FaHamburger /> },
  { href: "/admin/orders", label: "Orders", icon: <FaClipboardList /> },
  { href: "/admin/users", label: "Users", icon: <FaUsers /> },
  { href: "/admin/contact", label: "Feedback", icon: <FaComment /> },
];

export default function AdminSidebarLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const pathname = usePathname();
  const { user } = useAuth();
  const [sidebarOpen, setSidebarOpen] = useState(false);

  const links = user?.isSuperAdmin
    ? [...baseLinks, { href: "/admin/roles", label: "Manage Roles", icon: <FaUserShield /> }]
    : baseLinks;

  return (
    <div className="flex min-h-screen bg-gray-100 overflow-auto relative">
      <aside className="hidden md:flex w-64 bg-[#1a1a1a] text-white flex-col py-6 px-4 shadow-md z-40">
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

      <button
        className="md:hidden fixed top-3 left-3 z-50 text-xl text-white bg-black p-1.5 rounded shadow"
        onClick={() => setSidebarOpen(true)}
        aria-label="Open Sidebar"
      >
        <FaBars />
      </button>

      {sidebarOpen && (
        <>
          <div
            className="fixed inset-0 bg-black bg-opacity-50 z-40"
            onClick={() => setSidebarOpen(false)}
          />
          <aside className="fixed top-0 left-0 w-64 h-full bg-[#1a1a1a] text-white flex flex-col py-6 px-4 shadow-md z-50 transition-transform duration-300 transform">
            <div className="flex justify-between items-center mb-6">
              <h2 className="text-xl font-bold text-yellow-400 font-serif">MunchKing Admin</h2>
              <button
                onClick={() => setSidebarOpen(false)}
                aria-label="Close Sidebar"
                className="text-white text-lg"
              >
                <FaTimes />
              </button>
            </div>
            <nav className="flex flex-col gap-4">
              {links.map(({ href, label, icon }) => (
                <Link
                  key={href}
                  href={href}
                  onClick={() => setSidebarOpen(false)}
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
        </>
      )}

      <main className="flex-1 bg-gray-100 p-6 overflow-auto">{children}</main>
    </div>
  );
}
