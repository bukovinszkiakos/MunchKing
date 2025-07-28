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
import { apiGet } from "@/utils/api";

interface DashboardData {
  totalOrders: number;
  totalUsers: number;
  totalRevenue: number;
  totalFoodItems: number;
  ordersPerStatus: Record<string, number>;
  topSellingItems: {
    foodName: string;
    quantitySold: number;
  }[];
  totalFeedbacks: number;
  totalCategories: number;
}

export default function AdminDashboardPage() {
  const { user, logout } = useAuth();
  const router = useRouter();
  const dropdownRef = useRef<HTMLDivElement>(null);

  const [showDropdown, setShowDropdown] = useState(false);
  const [data, setData] = useState<DashboardData | null>(null);

  useEffect(() => {
    if (user && !user.isAdmin) {
      router.replace("/");
    }
  }, [user, router]);

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

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const stats = await apiGet<DashboardData>("/api/admin/dashboard/stats");
        setData(stats);
      } catch (err) {
        console.error("Failed to load dashboard stats", err);
      }
    };

    if (user?.isAdmin) fetchStats();
  }, [user]);

  if (!user) return null;

  return (
    <div className="min-h-screen bg-gray-100 p-4 sm:p-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6 relative">
        <h1 className="text-2xl sm:text-3xl font-bold text-yellow-600 flex items-center gap-2">
          📊 Admin Dashboard
        </h1>

        <div className="relative" ref={dropdownRef}>
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

      <div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-4 gap-4 sm:gap-6">
        <DashboardCard
          icon={<FaShoppingBasket className="text-blue-500 text-2xl sm:text-3xl" />}
          title="Categories"
          value={data?.totalCategories ?? 0}
          href="/admin/categories"
        />
        <DashboardCard
          icon={<FaHamburger className="text-red-400 text-2xl sm:text-3xl" />}
          title="Products"
          value={data?.totalFoodItems ?? 0}
          href="/admin/products"
        />
        <DashboardCard
          icon={<FaClipboardList className="text-green-600 text-2xl sm:text-3xl" />}
          title="Total Orders"
          value={data?.totalOrders ?? 0}
          href="/admin/orders"
        />
        <DashboardCard
          icon={<FaCheck className="text-yellow-500 text-2xl sm:text-3xl" />}
          title="Delivered"
          value={data?.ordersPerStatus?.Completed ?? 0}
          href="/admin/delivered"
        />
        <DashboardCard
          icon={<FaClock className="text-orange-400 text-2xl sm:text-3xl" />}
          title="Pending"
          value={data?.ordersPerStatus?.Pending ?? 0}
          href="/admin/pending"
        />
        <DashboardCard
          icon={<FaUsers className="text-indigo-500 text-2xl sm:text-3xl" />}
          title="Users"
          value={data?.totalUsers ?? 0}
          href="/admin/users"
        />
        <DashboardCard
          icon={<FaMoneyBill className="text-green-500 text-2xl sm:text-3xl" />}
          title="Sold Amount"
          value={new Intl.NumberFormat("en-US", {
            style: "currency",
            currency: "USD",
          }).format(data?.totalRevenue ?? 0)}
          href="/admin/sold"
        />
        <DashboardCard
          icon={<FaComments className="text-yellow-500 text-2xl sm:text-3xl" />}
          title="Feedbacks"
          value={data?.totalFeedbacks ?? 0}
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
    <div className="bg-white rounded-xl shadow-md p-4 flex flex-col justify-between hover:shadow-lg transition text-sm sm:text-base">
      <div className="flex items-center gap-3 mb-3">
        {icon}
        <h3 className="font-semibold text-gray-700">{title}</h3>
      </div>
      <div className="text-2xl sm:text-3xl font-bold text-gray-900 mb-3">{value}</div>
      <Link href={href} className="text-blue-600 hover:underline mt-auto text-sm">
        View Details →
      </Link>
    </div>
  );
}
