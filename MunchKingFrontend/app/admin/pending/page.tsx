"use client";

import { useEffect, useState } from "react";
import { apiGet, apiPut } from "@/utils/api";
import { FaChevronDown, FaChevronUp, FaArrowUp, FaArrowDown } from "react-icons/fa";
import Image from "next/image";
import React from "react";

interface OrderItem {
  productName: string;
  imageUrl: string;
  unitPrice: number;
  quantity: number;
}

interface Order {
  orderId: number;
  createdAt: string;
  paymentMode: string;
  userEmail: string;
  items: OrderItem[];
}

export default function PendingOrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [expandedId, setExpandedId] = useState<number | null>(null);
  const [sortKey, setSortKey] = useState<"createdAt" | "userEmail" | "paymentMode" | "total">("createdAt");
  const [sortAsc, setSortAsc] = useState(false);

  useEffect(() => {
    fetchPendingOrders();
  }, []);

  const fetchPendingOrders = async () => {
    try {
      const res = await apiGet<Order[]>("/api/orders/admin/all-orders?status=Pending");
      setOrders(res);
    } catch (err) {
      console.error("Failed to fetch pending orders", err);
    } finally {
      setLoading(false);
    }
  };

  const updateStatus = async (orderId: number, newStatus: string) => {
    try {
      await apiPut(`/api/orders/update-status/${orderId}`, { newStatus });
      fetchPendingOrders();
    } catch {
      alert("Failed to update order status.");
    }
  };

  const calculateTotal = (order: Order) =>
    order.items.reduce((sum, item) => sum + item.unitPrice * item.quantity, 0);

  const toggleSortOrder = () => setSortAsc((prev) => !prev);

  const sortedOrders = [...orders].sort((a, b) => {
    let valA: string | number = "";
    let valB: string | number = "";

    switch (sortKey) {
      case "createdAt":
        valA = new Date(a.createdAt).getTime();
        valB = new Date(b.createdAt).getTime();
        break;
      case "userEmail":
        valA = a.userEmail.toLowerCase();
        valB = b.userEmail.toLowerCase();
        break;
      case "paymentMode":
        valA = a.paymentMode.toLowerCase();
        valB = b.paymentMode.toLowerCase();
        break;
      case "total":
        valA = calculateTotal(a);
        valB = calculateTotal(b);
        break;
    }

    if (typeof valA === "number" && typeof valB === "number") {
      return sortAsc ? valA - valB : valB - valA;
    }

    if (typeof valA === "string" && typeof valB === "string") {
      return sortAsc ? valA.localeCompare(valB) : valB.localeCompare(valA);
    }

    return 0;
  });

  return (
    <div className="min-h-screen bg-gray-100 p-4 sm:p-6">
      <h1 className="text-2xl sm:text-3xl font-bold text-orange-500 mb-6">
        🕒 Pending Orders
      </h1>

      {loading ? (
        <p>Loading...</p>
      ) : (
        <>
          <div className="mb-4 flex flex-col sm:flex-row gap-3 sm:items-center justify-between">
            <p className="text-lg">
              Total Pending Orders:{" "}
              <strong className="text-orange-600">{orders.length}</strong>
            </p>

            <div className="flex gap-2 items-center">
              <label className="text-sm font-semibold">Sort by:</label>
              <select
                value={sortKey}
                onChange={(e) => setSortKey(e.target.value as any)}
                className="border px-2 py-1 rounded text-sm"
              >
                <option value="createdAt">Date</option>
                <option value="userEmail">User</option>
                <option value="paymentMode">Payment</option>
                <option value="total">Total</option>
              </select>
              <button
                onClick={toggleSortOrder}
                className="text-sm text-gray-600"
                title="Toggle sort order"
              >
                {sortAsc ? <FaArrowUp /> : <FaArrowDown />}
              </button>
            </div>
          </div>

          <div className="space-y-4">
            {sortedOrders.map((order) => (
              <div
                key={order.orderId}
                className="bg-white rounded-lg shadow p-4 space-y-3"
              >
                <div className="flex flex-col sm:flex-row justify-between sm:items-center">
                  <div className="space-y-1 text-sm">
                    <p className="font-mono text-gray-500">#{order.orderId}</p>
                    <p>{new Date(order.createdAt).toLocaleString("hu-HU")}</p>
                    <p>{order.userEmail}</p>
                    <p className="text-gray-600">
                      <span className="font-semibold">Payment:</span>{" "}
                      {order.paymentMode}
                    </p>
                    <p className="text-gray-600">
                      <span className="font-semibold">Total:</span>{" "}
                      ${calculateTotal(order).toFixed(2)}
                    </p>
                  </div>

                  <div className="flex flex-col sm:flex-row gap-2 mt-4 sm:mt-0">
                    <button
                      onClick={() => updateStatus(order.orderId, "InProgress")}
                      className="bg-blue-500 hover:bg-blue-600 text-white px-4 py-1.5 rounded text-sm font-medium transition"
                    >
                      Mark InProgress
                    </button>
                    <button
                      onClick={() => updateStatus(order.orderId, "Cancelled")}
                      className="bg-red-500 hover:bg-red-600 text-white px-4 py-1.5 rounded text-sm font-medium transition"
                    >
                      Cancel
                    </button>
                    <button
                      onClick={() =>
                        setExpandedId(
                          expandedId === order.orderId ? null : order.orderId
                        )
                      }
                      className="text-xl text-gray-600"
                      aria-label="Toggle Products"
                    >
                      {expandedId === order.orderId ? (
                        <FaChevronUp />
                      ) : (
                        <FaChevronDown />
                      )}
                    </button>
                  </div>
                </div>

                {expandedId === order.orderId && (
                  <div className="pt-4">
                    <p className="font-semibold mb-3">Products:</p>
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                      {order.items.map((item, i) => (
                        <div
                          key={i}
                          className="border p-3 rounded flex items-center gap-3 shadow-sm"
                        >
                          <Image
                            src={item.imageUrl}
                            alt={item.productName}
                            width={64}
                            height={64}
                            className="rounded object-cover"
                            unoptimized
                          />
                          <div className="text-sm">
                            <p className="font-semibold">{item.productName}</p>
                            <p>Qty: {item.quantity}</p>
                            <p>
                              Total: $
                              {(item.unitPrice * item.quantity).toFixed(2)}
                            </p>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            ))}
          </div>

          {orders.length === 0 && (
            <p className="text-center text-gray-500 mt-6">
              No pending orders found.
            </p>
          )}
        </>
      )}
    </div>
  );
}
