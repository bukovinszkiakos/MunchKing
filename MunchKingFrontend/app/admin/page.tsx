"use client";

import { useAuth } from "../context/AuthContext";
import { useRouter } from "next/navigation";
import { useEffect, useRef, useState } from "react";
import {
  FaShoppingBasket,
  FaHamburger,
  FaUsers,
  FaClipboardList,
  FaCheck,
  FaClock,
  FaMoneyBill,
  FaComments,
  FaChevronDown,
} from "react-icons/fa";
import Link from "next/link";

interface DashboardData {
  categories: number;
  products: number;
  totalOrders: number;
  delivered: number;
  pending: number;
  users: number;
  totalAmount: number;
  feedbacks: number;
}

export default function AdminDashboardPage() {
  const { user, logout } = useAuth();
  const router = useRouter();
  const dropdownRef = useRef<HTMLDivElement>(null);

  const [showDropdown, setShowDropdown] = useState(false);

  const [data, setData] = useState<DashboardData>({
    categories: 8,
    products: 14,
    totalOrders: 31,
    delivered: 9,
    pending: 22,
    users: 7,
    totalAmount: 6100,
    feedbacks: 3,
  });

  useEffect(() => {
    if (user && !user.isAdmin) {
      router.replace("/");
    }
  }, [user]);

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (
        dropdownRef.current &&
        !dropdownRef.current.contains(event.target as Node)
      ) {
        setShowDropdown(false);
      }
    };
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  if (!user) return null;

  return (
    <div className="min-h-screen bg-gray-100 p-6">
      <div className="flex justify-between items-start mb-4 relative">
        <h1 className="text-3xl font-bold text-yellow-600 flex items-center gap-2 mt-2">
          📊 Admin Dashboard
        </h1>

        <div className="relative mt-1" ref={dropdownRef}>
          <button
            onClick={() => setShowDropdown((prev) => !prev)}
            className="flex items-center gap-2 bg-yellow-400 text-black px-4 py-2 rounded-lg font-semibold shadow"
          >
            {user.username || user.email}
            <FaChevronDown className="text-sm" />
          </button>
          {showDropdown && (
            <div className="absolute right-0 mt-2 w-32 bg-white border border-gray-200 rounded-lg shadow-md z-10">
              <button
                onClick={logout}
                className="w-full text-left px-4 py-2 hover:bg-gray-100 text-sm"
              >
                Logout
              </button>
            </div>
          )}
        </div>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
        <DashboardCard
          icon={<FaShoppingBasket className="text-blue-500 text-3xl" />}
          title="Categories"
          value={data.categories}
          href="/admin/categories"
        />
        <DashboardCard
          icon={<FaHamburger className="text-red-400 text-3xl" />}
          title="Products"
          value={data.products}
          href="/admin/products"
        />
        <DashboardCard
          icon={<FaClipboardList className="text-green-600 text-3xl" />}
          title="Total Orders"
          value={data.totalOrders}
          href="/admin/orders"
        />
        <DashboardCard
          icon={<FaCheck className="text-yellow-500 text-3xl" />}
          title="Delivered Items"
          value={data.delivered}
          href="/admin/orders"
        />
        <DashboardCard
          icon={<FaClock className="text-orange-400 text-3xl" />}
          title="Pending Items"
          value={data.pending}
          href="/admin/orders"
        />
        <DashboardCard
          icon={<FaUsers className="text-indigo-500 text-3xl" />}
          title="Users"
          value={data.users}
          href="/admin/users"
        />
        <DashboardCard
          icon={<FaMoneyBill className="text-green-500 text-3xl" />}
          title="Sold Amount"
          value={`₹${data.totalAmount}`}
          href="/admin/orders"
        />
        <DashboardCard
          icon={<FaComments className="text-yellow-500 text-3xl" />}
          title="Feedbacks"
          value={data.feedbacks}
          href="/admin/contact"
        />
      </div>
    </div>
  );
}

function DashboardCard({
  icon,
  title,
  value,
  href,
}: {
  icon: React.ReactNode;
  title: string;
  value: number | string;
  href: string;
}) {
  return (
    <div className="bg-white rounded-xl shadow-md p-5 flex flex-col justify-between hover:shadow-lg transition">
      <div className="flex items-center gap-4 mb-4">
        {icon}
        <h3 className="text-lg font-semibold text-gray-700">{title}</h3>
      </div>
      <div className="text-3xl font-bold text-gray-900 mb-4">{value}</div>
      <Link href={href} className="text-sm text-blue-600 hover:underline mt-auto">
        View Details →
      </Link>
    </div>
  );
}
