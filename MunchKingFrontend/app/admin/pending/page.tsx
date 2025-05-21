"use client";

import { useEffect, useState } from "react";
import { apiGet, apiPut } from "@/utils/api";
import { FaChevronDown, FaChevronUp } from "react-icons/fa";
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

  return (
    <div className="min-h-screen bg-gray-100 p-6">
      <h1 className="text-3xl font-bold text-orange-500 mb-6">🕒 Pending Orders</h1>

      {loading ? (
        <p>Loading...</p>
      ) : (
        <>
          <p className="mb-4 text-lg">
            Total Pending Orders: <strong>{orders.length}</strong>
          </p>

          <div className="bg-white rounded-lg shadow overflow-x-auto">
            <table className="w-full text-sm text-left">
              <thead className="bg-gray-200">
                <tr>
                  <th className="p-3"></th>
                  <th className="p-3">Date</th>
                  <th className="p-3">User</th>
                  <th className="p-3">Payment</th>
                  <th className="p-3">Actions</th>
                </tr>
              </thead>
              <tbody>
                {orders.map((order) => (
                  <React.Fragment key={order.orderId}>
                    <tr className="border-t">
                      <td className="p-3">
                        <button onClick={() => setExpandedId(expandedId === order.orderId ? null : order.orderId)}>
                          {expandedId === order.orderId ? <FaChevronUp /> : <FaChevronDown />}
                        </button>
                      </td>
                      <td className="p-3">{new Date(order.createdAt).toLocaleString()}</td>
                      <td className="p-3">{order.userEmail}</td>
                      <td className="p-3">{order.paymentMode}</td>
                      <td className="p-3 space-x-2">
                        <button
                          onClick={() => updateStatus(order.orderId, "InProgress")}
                          className="bg-blue-500 text-white px-2 py-1 rounded text-sm hover:bg-blue-600"
                        >
                          Mark InProgress
                        </button>
                        <button
                          onClick={() => updateStatus(order.orderId, "Cancelled")}
                          className="bg-red-500 text-white px-2 py-1 rounded text-sm hover:bg-red-600"
                        >
                          Cancel
                        </button>
                      </td>
                    </tr>

                    {expandedId === order.orderId && (
                      <tr className="bg-gray-50 border-t">
                        <td colSpan={5} className="p-4">
                          <p className="font-semibold mb-2">Products:</p>
                          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
                            {order.items.map((item, i) => (
                              <div key={i} className="border p-3 rounded shadow-sm flex gap-3 items-center">
                                <img src={item.imageUrl} alt={item.productName} className="w-14 h-14 object-cover rounded" />
                                <div>
                                  <p className="font-semibold">{item.productName}</p>
                                  <p className="text-sm">Qty: {item.quantity}</p>
                                  <p className="text-sm">
                                    Total: ${(item.unitPrice * item.quantity).toFixed(2)}
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

                {orders.length === 0 && (
                  <tr>
                    <td colSpan={5} className="text-center py-6 text-gray-500">
                      No pending orders found.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </>
      )}
    </div>
  );
}
