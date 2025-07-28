"use client";

import { useEffect, useState } from "react";
import { apiGet } from "@/utils/api";
import { FaChevronUp, FaChevronDown } from "react-icons/fa";

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

interface SalesSummary {
  totalOrders: number;
  totalRevenue: number;
  revenueByDay: Record<string, number>;
  ordersByDay: Record<string, number>;
  revenueByWeek: Record<string, number>;
  revenueByMonth: Record<string, number>;
}

interface TopProduct {
  foodName: string;
  quantitySold: number;
}

export default function SoldAmountPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [summary, setSummary] = useState<SalesSummary | null>(null);
  const [topProducts, setTopProducts] = useState<TopProduct[]>([]);
  const [loading, setLoading] = useState(true);

  const [completedSortKey, setCompletedSortKey] = useState<"createdAt" | "userEmail" | "paymentMode" | "total">("createdAt");
  const [completedSortAsc, setCompletedSortAsc] = useState(true);

  const [dailySortKey, setDailySortKey] = useState<"date" | "orders" | "revenue">("date");
  const [dailySortAsc, setDailySortAsc] = useState(true);

  const [weeklySortKey, setWeeklySortKey] = useState<"week" | "revenue">("week");
  const [weeklySortAsc, setWeeklySortAsc] = useState(true);

  const [monthlySortKey, setMonthlySortKey] = useState<"month" | "revenue">("month");
  const [monthlySortAsc, setMonthlySortAsc] = useState(true);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const [ordersRes, summaryRes, topRes] = await Promise.all([
        apiGet<Order[]>("/api/orders/admin/all-orders?status=Completed"),
        apiGet<SalesSummary>("/api/admin/stats/sales-summary"),
        apiGet<TopProduct[]>("/api/admin/stats/top-products"),
      ]);
      setOrders(ordersRes);
      setSummary(summaryRes);
      setTopProducts(topRes);
    } catch (err) {
      console.error("Failed to fetch sales data", err);
    } finally {
      setLoading(false);
    }
  };

  const renderArrow = (asc: boolean, active: boolean) => (
    <span className="ml-1 w-4 inline-block">
      {active && (asc ? <FaChevronUp /> : <FaChevronDown />)}
    </span>
  );

  const getTotal = (order: Order) =>
    order.items.reduce((sum, item) => sum + item.unitPrice * item.quantity, 0);

  const sortedCompletedOrders = [...orders].sort((a, b) => {
    const aVal = completedSortKey === "total" ? getTotal(a) : a[completedSortKey];
    const bVal = completedSortKey === "total" ? getTotal(b) : b[completedSortKey];
    return completedSortAsc
      ? `${aVal}`.localeCompare(`${bVal}`, undefined, { numeric: true })
      : `${bVal}`.localeCompare(`${aVal}`, undefined, { numeric: true });
  });

  const sortedDaily = summary
    ? Object.entries(summary.revenueByDay).sort(([dateA], [dateB]) => {
        const aVal = dailySortKey === "date" ? dateA : dailySortKey === "orders" ? summary.ordersByDay[dateA] : summary.revenueByDay[dateA];
        const bVal = dailySortKey === "date" ? dateB : dailySortKey === "orders" ? summary.ordersByDay[dateB] : summary.revenueByDay[dateB];
        return dailySortAsc
          ? `${aVal}`.localeCompare(`${bVal}`, undefined, { numeric: true })
          : `${bVal}`.localeCompare(`${aVal}`, undefined, { numeric: true });
      })
    : [];

  const sortedWeekly = summary
    ? Object.entries(summary.revenueByWeek).sort(([a, valA], [b, valB]) => {
        const left = weeklySortKey === "week" ? a : valA;
        const right = weeklySortKey === "week" ? b : valB;
        return weeklySortAsc
          ? `${left}`.localeCompare(`${right}`, undefined, { numeric: true })
          : `${right}`.localeCompare(`${left}`, undefined, { numeric: true });
      })
    : [];

  const sortedMonthly = summary
    ? Object.entries(summary.revenueByMonth).sort(([a, valA], [b, valB]) => {
        const left = monthlySortKey === "month" ? a : valA;
        const right = monthlySortKey === "month" ? b : valB;
        return monthlySortAsc
          ? `${left}`.localeCompare(`${right}`, undefined, { numeric: true })
          : `${right}`.localeCompare(`${left}`, undefined, { numeric: true });
      })
    : [];

  return (
    <div className="min-h-screen bg-gray-100 p-6">
      <h1 className="text-3xl font-bold text-green-700 mb-6">💰 Sales Dashboard</h1>

      {loading ? (
        <p>Loading...</p>
      ) : (
        <>
          {summary && (
            <div className="mb-6 text-lg bg-white rounded shadow p-4">
              <p>Number of Completed Orders: <strong>{summary.totalOrders}</strong></p>
              <p>Total Revenue: <strong>{summary.totalRevenue.toFixed(2)} USD</strong></p>
            </div>
          )}

          <div className="bg-white rounded shadow p-4 mb-6">
            <h2 className="text-xl font-semibold mb-4">📆 Daily Revenue</h2>
            <table className="w-full text-sm text-left">
              <thead className="bg-gray-200">
                <tr>
                  <th className="p-2 cursor-pointer" onClick={() => { setDailySortKey("date"); setDailySortAsc(!dailySortAsc); }}>
                    <span className="inline-flex items-center">Date {renderArrow(dailySortAsc, dailySortKey === "date")}</span>
                  </th>
                  <th className="p-2 cursor-pointer" onClick={() => { setDailySortKey("orders"); setDailySortAsc(!dailySortAsc); }}>
                    <span className="inline-flex items-center">Orders {renderArrow(dailySortAsc, dailySortKey === "orders")}</span>
                  </th>
                  <th className="p-2 cursor-pointer" onClick={() => { setDailySortKey("revenue"); setDailySortAsc(!dailySortAsc); }}>
                    <span className="inline-flex items-center">Revenue {renderArrow(dailySortAsc, dailySortKey === "revenue")}</span>
                  </th>
                </tr>
              </thead>
              <tbody>
                {sortedDaily.map(([date, revenue]) => (
                  <tr key={date} className="border-t">
                    <td className="p-2">{date}</td>
                    <td className="p-2">{summary?.ordersByDay[date]}</td>
                    <td className="p-2">${revenue.toFixed(2)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="bg-white rounded shadow p-4 mb-6">
            <h2 className="text-xl font-semibold mb-4">📅 Weekly Revenue</h2>
            <table className="w-full text-sm text-left">
              <thead className="bg-gray-200">
                <tr>
                  <th className="p-2 cursor-pointer" onClick={() => { setWeeklySortKey("week"); setWeeklySortAsc(!weeklySortAsc); }}>
                    <span className="inline-flex items-center">Week {renderArrow(weeklySortAsc, weeklySortKey === "week")}</span>
                  </th>
                  <th className="p-2 cursor-pointer" onClick={() => { setWeeklySortKey("revenue"); setWeeklySortAsc(!weeklySortAsc); }}>
                    <span className="inline-flex items-center">Revenue {renderArrow(weeklySortAsc, weeklySortKey === "revenue")}</span>
                  </th>
                </tr>
              </thead>
              <tbody>
                {sortedWeekly.map(([week, revenue]) => (
                  <tr key={week} className="border-t">
                    <td className="p-2">{week}</td>
                    <td className="p-2">${revenue.toFixed(2)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="bg-white rounded shadow p-4 mb-6">
            <h2 className="text-xl font-semibold mb-4">📅 Monthly Revenue</h2>
            <table className="w-full text-sm text-left">
              <thead className="bg-gray-200">
                <tr>
                  <th className="p-2 cursor-pointer" onClick={() => { setMonthlySortKey("month"); setMonthlySortAsc(!monthlySortAsc); }}>
                    <span className="inline-flex items-center">Month {renderArrow(monthlySortAsc, monthlySortKey === "month")}</span>
                  </th>
                  <th className="p-2 cursor-pointer" onClick={() => { setMonthlySortKey("revenue"); setMonthlySortAsc(!monthlySortAsc); }}>
                    <span className="inline-flex items-center">Revenue {renderArrow(monthlySortAsc, monthlySortKey === "revenue")}</span>
                  </th>
                </tr>
              </thead>
              <tbody>
                {sortedMonthly.map(([month, revenue]) => (
                  <tr key={month} className="border-t">
                    <td className="p-2">{month}</td>
                    <td className="p-2">${revenue.toFixed(2)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="bg-white rounded shadow p-4">
            <h2 className="text-xl font-semibold mb-4">📦 Completed Orders</h2>
            <table className="w-full text-sm text-left">
              <thead className="bg-gray-200">
                <tr>
                  <th className="p-2">#</th>
                  <th className="p-2 cursor-pointer" onClick={() => { setCompletedSortKey("createdAt"); setCompletedSortAsc(!completedSortAsc); }}>
                    <span className="inline-flex items-center">Date {renderArrow(completedSortAsc, completedSortKey === "createdAt")}</span>
                  </th>
                  <th className="p-2 cursor-pointer" onClick={() => { setCompletedSortKey("userEmail"); setCompletedSortAsc(!completedSortAsc); }}>
                    <span className="inline-flex items-center">User {renderArrow(completedSortAsc, completedSortKey === "userEmail")}</span>
                  </th>
                  <th className="p-2 cursor-pointer" onClick={() => { setCompletedSortKey("paymentMode"); setCompletedSortAsc(!completedSortAsc); }}>
                    <span className="inline-flex items-center">Payment {renderArrow(completedSortAsc, completedSortKey === "paymentMode")}</span>
                  </th>
                  <th className="p-2 cursor-pointer" onClick={() => { setCompletedSortKey("total"); setCompletedSortAsc(!completedSortAsc); }}>
                    <span className="inline-flex items-center">Total {renderArrow(completedSortAsc, completedSortKey === "total")}</span>
                  </th>
                </tr>
              </thead>
              <tbody>
                {sortedCompletedOrders.map((order) => (
                  <tr key={order.orderId} className="border-t">
                    <td className="p-2 font-mono">#{order.orderId}</td>
                    <td className="p-2">{new Date(order.createdAt).toLocaleString("hu-HU")}</td>
                    <td className="p-2">{order.userEmail}</td>
                    <td className="p-2">{order.paymentMode}</td>
                    <td className="p-2 font-semibold">${getTotal(order).toFixed(2)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </>
      )}
    </div>
  );
}
