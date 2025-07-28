"use client";

import { useEffect, useState } from "react";
import { apiGet } from "@/utils/api";
import { FaArrowUp, FaArrowDown } from "react-icons/fa6";

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

export default function DeliveredOrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [sortKey, setSortKey] = useState<"orderId" | "createdAt" | "userEmail" | "paymentMode" | "total">("createdAt");
  const [sortAsc, setSortAsc] = useState(false);

  useEffect(() => {
    fetchDeliveredOrders();
  }, []);

  const fetchDeliveredOrders = async () => {
    try {
      const res = await apiGet<Order[]>("/api/orders/admin/all-orders?status=Completed");
      setOrders(res);
    } catch (err) {
      console.error("Failed to fetch delivered orders", err);
    } finally {
      setLoading(false);
    }
  };

  const calculateTotal = (order: Order) =>
    order.items.reduce((sum, item) => sum + item.unitPrice * item.quantity, 0);

  const toggleSort = (key: typeof sortKey) => {
    if (key === sortKey) {
      setSortAsc(!sortAsc);
    } else {
      setSortKey(key);
      setSortAsc(true);
    }
  };

  const renderArrow = (key: typeof sortKey) => {
    if (sortKey !== key) return null;
    return sortAsc ? <FaArrowUp className="inline ml-1" /> : <FaArrowDown className="inline ml-1" />;
  };

  const sortedOrders = [...orders].sort((a, b) => {
    let valA: string | number = "";
    let valB: string | number = "";

    switch (sortKey) {
      case "orderId":
        valA = a.orderId;
        valB = b.orderId;
        break;
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

  const totalRevenue = orders.reduce((sum, order) => sum + calculateTotal(order), 0);

  return (
    <div className="min-h-screen bg-gray-100 p-6">
      <h1 className="text-3xl font-bold text-green-600 mb-6">✅ Delivered Orders</h1>

      {loading ? (
        <p>Loading...</p>
      ) : (
        <>
          <div className="mb-6">
            <p className="text-lg">
              Total Delivered Orders: <strong>{orders.length}</strong>
            </p>
            <p className="text-lg">
              Total Revenue: <strong>${totalRevenue.toFixed(2)}</strong>
            </p>
          </div>

          <div className="grid gap-4 sm:hidden">
            {orders.length === 0 ? (
              <div className="text-center text-gray-500">No delivered orders found.</div>
            ) : (
              sortedOrders.map((order) => (
                <div
                  key={order.orderId}
                  className="bg-white rounded-lg shadow p-4 text-sm space-y-1"
                >
                  <p className="font-bold text-gray-600">#{order.orderId}</p>
                  <p className="text-gray-600">
                    {new Date(order.createdAt).toLocaleString()}
                  </p>
                  <p>
                    <span className="font-semibold">User:</span> {order.userEmail}
                  </p>
                  <p>
                    <span className="font-semibold">Payment:</span> {order.paymentMode}
                  </p>
                  <p className="font-semibold">Total: ${calculateTotal(order).toFixed(2)}</p>
                </div>
              ))
            )}
          </div>

          <div className="hidden sm:block bg-white rounded-lg shadow overflow-x-auto">
            <table className="w-full text-sm text-left">
              <thead className="bg-gray-200">
                <tr>
                  <th className="px-4 py-3 cursor-pointer" onClick={() => toggleSort("orderId")}>
                    Order ID{renderArrow("orderId")}
                  </th>
                  <th className="px-4 py-3 cursor-pointer" onClick={() => toggleSort("createdAt")}>
                    Date{renderArrow("createdAt")}
                  </th>
                  <th className="px-4 py-3 cursor-pointer" onClick={() => toggleSort("userEmail")}>
                    User{renderArrow("userEmail")}
                  </th>
                  <th className="px-4 py-3 cursor-pointer" onClick={() => toggleSort("paymentMode")}>
                    Payment{renderArrow("paymentMode")}
                  </th>
                  <th className="px-4 py-3 cursor-pointer" onClick={() => toggleSort("total")}>
                    Total{renderArrow("total")}
                  </th>
                </tr>
              </thead>
              <tbody>
                {sortedOrders.map((order) => (
                  <tr key={order.orderId} className="border-t">
                    <td className="px-4 py-3 font-mono">#{order.orderId}</td>
                    <td className="px-4 py-3">
                      {new Date(order.createdAt).toLocaleString()}
                    </td>
                    <td className="px-4 py-3">{order.userEmail}</td>
                    <td className="px-4 py-3">{order.paymentMode}</td>
                    <td className="px-4 py-3 font-semibold">
                      ${calculateTotal(order).toFixed(2)}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>

            {orders.length === 0 && (
              <div className="p-6 text-center text-gray-500">No delivered orders found.</div>
            )}
          </div>
        </>
      )}
    </div>
  );
}
