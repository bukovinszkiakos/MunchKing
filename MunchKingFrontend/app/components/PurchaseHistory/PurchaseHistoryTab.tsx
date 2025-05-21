"use client";

import { useEffect, useState } from "react";
import { apiDelete, apiGet } from "@/utils/api";
import { useAuth } from "../../context/AuthContext";

interface OrderItem {
  productName: string;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
}

interface Order {
  orderId: number;
  createdAt: string;
  status: string;
  paymentMode: string;
  items: OrderItem[];
}

const statusStyles: Record<string, string> = {
  Pending: "bg-orange-500 text-white",
  Completed: "bg-green-600 text-white",
  InProgress: "bg-blue-500 text-white",
  Cancelled: "bg-gray-500 text-white",
};

export default function PurchaseHistoryTab() {
  const { user } = useAuth();
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function fetchOrders() {
      try {
        const res = await apiGet<Order[]>("/api/orders/my-orders");
        setOrders(res);
      } catch (err) {
        console.error("Failed to fetch orders", err);
      } finally {
        setLoading(false);
      }
    }
    fetchOrders();
  }, []);

  const handleDelete = async (orderId: number) => {
    const confirmed = window.confirm("Are you sure you want to delete this order?");
    if (!confirmed) return;

    try {
      await apiDelete(`/api/orders/${orderId}`);
      setOrders((prev) => prev.filter((o) => o.orderId !== orderId));
    } catch (err) {
      console.error("Failed to delete order", err);
    }
  };

  if (loading) return <p className="mt-10 text-center">Loading orders...</p>;

  if (orders.length === 0)
    return <p className="mt-10 text-center">You have no purchase history yet.</p>;

  return (
    <div className="mt-8 max-h-[600px] overflow-y-auto pr-2">
      {orders.map((order) => (
        <div key={order.orderId} className="mb-8 border rounded-lg shadow p-6 bg-white">
          <div className="mb-4 flex justify-between items-center">
            <h2 className="font-bold text-lg">Order #{order.orderId}</h2>
            <div className="flex gap-2 items-center">
              <span
                className={`px-3 py-1 text-sm rounded-full font-semibold ${
                  statusStyles[order.status] || "bg-gray-400 text-white"
                }`}
              >
                {order.status}
              </span>
              <button
                onClick={() => handleDelete(order.orderId)}
                className="ml-2 px-3 py-1 bg-red-500 text-white text-sm rounded hover:bg-red-600"
              >
                Delete
              </button>
            </div>
          </div>

          <p className="text-sm text-gray-500 mb-2">
            Payment Mode: {order.paymentMode}
          </p>
          <p className="text-sm text-gray-500 mb-4">
            Ordered at: {new Date(order.createdAt).toLocaleString()}
          </p>

          <table className="w-full text-left border-t border-gray-200">
            <thead>
              <tr className="text-sm text-gray-700">
                <th className="py-2">Product</th>
                <th className="py-2">Unit Price</th>
                <th className="py-2">Qty</th>
                <th className="py-2">Total</th>
              </tr>
            </thead>
            <tbody>
              {order.items.map((item, idx) => (
                <tr key={idx} className="text-sm text-gray-800 border-t">
                  <td className="py-2 font-medium">{item.productName}</td>
                  <td className="py-2">${item.unitPrice.toFixed(2)}</td>
                  <td className="py-2">{item.quantity}</td>
                  <td className="py-2 font-semibold">
                    ${(item.unitPrice * item.quantity).toFixed(2)}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          <div className="mt-4 text-right font-bold">
            Grand Total: $
            {order.items
              .reduce((sum, item) => sum + item.unitPrice * item.quantity, 0)
              .toFixed(2)}
          </div>
        </div>
      ))}
    </div>
  );
}
