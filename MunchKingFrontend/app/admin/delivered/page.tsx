"use client";

import { useEffect, useState } from "react";
import { apiGet } from "@/utils/api";

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

  const totalRevenue = orders.reduce((sum, order) => {
    const orderTotal = order.items.reduce(
      (subtotal, item) => subtotal + item.unitPrice * item.quantity,
      0
    );
    return sum + orderTotal;
  }, 0);

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

          <div className="bg-white rounded-lg shadow overflow-x-auto">
            <table className="w-full text-sm text-left">
              <thead className="bg-gray-200">
                <tr>
                  <th className="p-3">#</th>
                  <th className="p-3">Date</th>
                  <th className="p-3">User</th>
                  <th className="p-3">Payment</th>
                  <th className="p-3">Total</th>
                </tr>
              </thead>
              <tbody>
                {orders.map((order, i) => {
                  const orderTotal = order.items.reduce(
                    (sum, item) => sum + item.unitPrice * item.quantity,
                    0
                  );
                  return (
                    <tr key={order.orderId} className="border-t">
                      <td className="p-3 font-mono">#{order.orderId}</td>
                      <td className="p-3">{new Date(order.createdAt).toLocaleString()}</td>
                      <td className="p-3">{order.userEmail}</td>
                      <td className="p-3">{order.paymentMode}</td>
                      <td className="p-3 font-semibold">${orderTotal.toFixed(2)}</td>
                    </tr>
                  );
                })}
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
