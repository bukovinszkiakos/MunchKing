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

export default function SoldAmountPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetchCompletedOrders();
  }, []);

  const fetchCompletedOrders = async () => {
    try {
      const res = await apiGet<Order[]>(
        "/api/orders/admin/all-orders?status=Completed"
      );
      setOrders(res);
    } catch (err) {
      console.error("Failed to fetch completed orders", err);
    } finally {
      setLoading(false);
    }
  };

  const totalAmount = orders.reduce(
    (sum, o) =>
      sum +
      o.items.reduce(
        (orderSum, item) => orderSum + item.unitPrice * item.quantity,
        0
      ),
    0
  );

  return (
    <div className="min-h-screen bg-gray-100 p-6">
      <h1 className="text-3xl font-bold text-green-700 mb-6">
        💰 Total Sold Amount
      </h1>

      {loading ? (
        <p>Loading...</p>
      ) : (
        <>
          <div className="mb-6 text-lg">
            <p>
              Number of Completed Orders: <strong>{orders.length}</strong>
            </p>
            <p className="text-lg">
              Total Revenue:{" "}
              <strong>
                {new Intl.NumberFormat("en-US", {
                  style: "currency",
                  currency: "USD",
                }).format(totalAmount)}
              </strong>
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
                {orders.map((order) => {
                  const orderTotal = order.items.reduce(
                    (sum, item) => sum + item.unitPrice * item.quantity,
                    0
                  );
                  return (
                    <tr key={order.orderId} className="border-t">
                      <td className="p-3 font-mono">#{order.orderId}</td>
                      <td className="p-3">
                        {new Date(order.createdAt).toLocaleString()}
                      </td>
                      <td className="p-3">{order.userEmail}</td>
                      <td className="p-3">{order.paymentMode}</td>
                      <td className="p-3 font-semibold">
                        {new Intl.NumberFormat("en-US", {
                          style: "currency",
                          currency: "USD",
                        }).format(orderTotal)}
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>

            {orders.length === 0 && (
              <div className="p-6 text-center text-gray-500">
                No completed orders yet.
              </div>
            )}
          </div>
        </>
      )}
    </div>
  );
}
