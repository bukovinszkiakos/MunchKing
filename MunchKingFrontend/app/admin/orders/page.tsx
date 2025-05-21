"use client";

import { useEffect, useState } from "react";
import { apiGet, apiPut } from "@/utils/api";
import { FaChevronDown, FaChevronUp } from "react-icons/fa";
import { FaArrowDownAZ, FaArrowUpZA } from "react-icons/fa6";
import React from "react";

interface OrderItem {
  productName: string;
  imageUrl: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
}

interface Order {
  orderId: number;
  createdAt: string;
  status: string;
  paymentMode: string;
  userEmail: string;
  username: string;
  items: OrderItem[];
}

const statusColors: Record<string, string> = {
  Pending: "bg-yellow-200 text-yellow-800",
  InProgress: "bg-blue-200 text-blue-800",
  Completed: "bg-green-200 text-green-800",
  Cancelled: "bg-gray-300 text-gray-700",
};

export default function AdminOrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [expandedId, setExpandedId] = useState<number | null>(null);
  const [selectedStatus, setSelectedStatus] = useState("");
  const [statusOptions, setStatusOptions] = useState<string[]>([]);
  const [sortKey, setSortKey] = useState<"orderId" | "createdAt" | "status">("createdAt");
  const [sortAsc, setSortAsc] = useState(false);

  useEffect(() => {
    fetchOrders();
    fetchStatuses();
  }, [selectedStatus]);

  const fetchOrders = async () => {
    const res = await apiGet<Order[]>(
      `/api/orders/admin/all-orders${selectedStatus ? `?status=${selectedStatus}` : ""}`
    );
    setOrders(res);
  };

  const fetchStatuses = async () => {
    const res = await apiGet<string[]>("/api/orders/available-statuses");
    setStatusOptions(res);
  };

  const updateStatus = async (orderId: number, newStatus: string) => {
    try {
      await apiPut(`/api/orders/update-status/${orderId}`, { newStatus });
      await fetchOrders();
    } catch {
      alert("Failed to update status.");
    }
  };

  const formatDate = (iso: string) =>
    new Date(iso).toLocaleString("en-US", { dateStyle: "medium", timeStyle: "short" });

  const sortedOrders = [...orders].sort((a, b) => {
    const valA = a[sortKey];
    const valB = b[sortKey];
    if (valA < valB) return sortAsc ? -1 : 1;
    if (valA > valB) return sortAsc ? 1 : -1;
    return 0;
  });

  const toggleSort = (key: typeof sortKey) => {
    if (key === sortKey) setSortAsc(!sortAsc);
    else {
      setSortKey(key);
      setSortAsc(true);
    }
  };

  return (
    <div className="min-h-screen bg-gray-100 p-6">
      <h1 className="text-3xl font-bold text-yellow-600 mb-6">📦 Manage Orders</h1>

      <div className="mb-4 flex items-center gap-4">
        <label className="font-semibold">Filter by Status:</label>
        <select
          className="border px-3 py-2 rounded"
          value={selectedStatus}
          onChange={(e) => setSelectedStatus(e.target.value)}
        >
          <option value="">All</option>
          {statusOptions.map((s) => (
            <option key={s} value={s}>
              {s}
            </option>
          ))}
        </select>
      </div>

      <div className="bg-white shadow rounded-lg overflow-x-auto">
        <table className="w-full text-sm border-collapse">
          <thead className="bg-gray-200">
            <tr>
              <th className="p-3"></th>
              <th className="p-3 cursor-pointer" onClick={() => toggleSort("orderId")}>
                Order ID {sortKey === "orderId" && (sortAsc ? <FaArrowUpZA /> : <FaArrowDownAZ />)}
              </th>
              <th className="p-3 cursor-pointer" onClick={() => toggleSort("createdAt")}>
                Date {sortKey === "createdAt" && (sortAsc ? <FaArrowUpZA /> : <FaArrowDownAZ />)}
              </th>
              <th className="p-3 cursor-pointer" onClick={() => toggleSort("status")}>
                Status {sortKey === "status" && (sortAsc ? <FaArrowUpZA /> : <FaArrowDownAZ />)}
              </th>
              <th className="p-3">Payment</th>
              <th className="p-3">User</th>
              <th className="p-3">Edit</th>
            </tr>
          </thead>
          <tbody>
            {sortedOrders.map((order) => (
              <React.Fragment key={order.orderId}>
                <tr className="border-t">
                  <td className="p-3">
                    <button
                      onClick={() =>
                        setExpandedId(expandedId === order.orderId ? null : order.orderId)
                      }
                    >
                      {expandedId === order.orderId ? <FaChevronUp /> : <FaChevronDown />}
                    </button>
                  </td>
                  <td className="p-3 font-mono text-xs">#{order.orderId}</td>
                  <td className="p-3">{formatDate(order.createdAt)}</td>
                  <td className="p-3">
                    <span
                      className={`text-xs font-semibold px-2 py-1 rounded-full ${
                        statusColors[order.status] || "bg-gray-200"
                      }`}
                    >
                      {order.status}
                    </span>
                  </td>
                  <td className="p-3">{order.paymentMode}</td>
                  <td className="p-3">{order.userEmail}</td>
                  <td className="p-3">
                    <select
                      value={order.status}
                      onChange={(e) => updateStatus(order.orderId, e.target.value)}
                      className="border text-sm px-2 py-1 rounded"
                    >
                      {statusOptions.map((s) => (
                        <option key={s} value={s}>
                          {s}
                        </option>
                      ))}
                    </select>
                  </td>
                </tr>
                {expandedId === order.orderId && (
                  <tr className="bg-gray-50">
                    <td colSpan={7} className="p-4">
                      <p className="font-semibold mb-2">Products:</p>
                      <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
                        {order.items.map((item, i) => (
                          <div
                            key={`${order.orderId}-${i}`}
                            className="border p-3 rounded shadow-sm flex gap-3 items-center"
                          >
                            <img
                              src={item.imageUrl}
                              alt={item.productName}
                              className="w-14 h-14 object-cover rounded"
                            />
                            <div>
                              <p className="font-semibold">{item.productName}</p>
                              <p className="text-sm">Qty: {item.quantity}</p>
                              <p className="text-sm">
                                ${(item.unitPrice * item.quantity).toFixed(2)}
                              </p>
                            </div>
                          </div>
                        ))}
                      </div>
                    </td>
                  </tr>
                )}
              </React.Fragment>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
