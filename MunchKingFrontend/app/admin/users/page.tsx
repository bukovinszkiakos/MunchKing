"use client";

import { useEffect, useState } from "react";
import { apiDelete, apiGet } from "@/utils/api";
import { FaTrash } from "react-icons/fa";

interface User {
  id: string;
  fullName: string;
  username: string;
  email: string;
  createdAt: string;
}

export default function AdminUsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [sortKey, setSortKey] = useState<keyof User>("createdAt");
  const [sortAsc, setSortAsc] = useState(true);
  const [search, setSearch] = useState("");

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      const res = await apiGet<User[]>("/api/admin/users");
      setUsers(res);
    } catch (err) {
      console.error("Failed to fetch users", err);
    }
  };

  const deleteUser = async (id: string) => {
    if (!confirm("Are you sure you want to delete this user?")) return;
    try {
      await apiDelete(`/api/admin/users/${id}`);
      fetchUsers();
    } catch {
      alert("Failed to delete user");
    }
  };

  const sortedUsers = [...users]
    .filter(
      (u) =>
        u.fullName.toLowerCase().includes(search.toLowerCase()) ||
        u.email.toLowerCase().includes(search.toLowerCase())
    )
    .sort((a, b) => {
      const aVal = a[sortKey];
      const bVal = b[sortKey];
      return sortAsc ? `${aVal}`.localeCompare(`${bVal}`) : `${bVal}`.localeCompare(`${aVal}`);
    });

  const toggleSort = (key: keyof User) => {
    if (key === sortKey) setSortAsc(!sortAsc);
    else {
      setSortKey(key);
      setSortAsc(true);
    }
  };

  return (
    <div className="min-h-screen bg-gray-100 p-6">
      <h1 className="text-3xl font-bold text-yellow-600 mb-6">👥 Manage Users</h1>

      <div className="mb-4 flex justify-between items-center">
        <div className="text-lg font-semibold">USERS LIST</div>
        <input
          type="text"
          placeholder="Search..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="border px-3 py-2 rounded shadow-sm"
        />
      </div>

      <div className="overflow-x-auto bg-white shadow rounded-lg">
        <table className="w-full text-sm text-left">
          <thead className="bg-gray-200">
            <tr>
              <th className="p-3">#</th>
              <th className="p-3 cursor-pointer" onClick={() => toggleSort("fullName")}>Full Name</th>
              <th className="p-3 cursor-pointer" onClick={() => toggleSort("username")}>Username</th>
              <th className="p-3 cursor-pointer" onClick={() => toggleSort("email")}>Email</th>
              <th className="p-3 cursor-pointer" onClick={() => toggleSort("createdAt")}>Joined Date</th>
              <th className="p-3">Delete</th>
            </tr>
          </thead>
          <tbody>
            {sortedUsers.map((user, index) => (
              <tr key={user.id} className="border-t">
                <td className="p-3">{index + 1}</td>
                <td className="p-3">{user.fullName}</td>
                <td className="p-3">{user.username}</td>
                <td className="p-3">{user.email}</td>
                <td className="p-3">{new Date(user.createdAt).toLocaleString()}</td>
                <td className="p-3">
                  <button
                    onClick={() => deleteUser(user.id)}
                    className="text-red-600 hover:text-red-800"
                  >
                    <FaTrash />
                  </button>
                </td>
              </tr>
            ))}
            {sortedUsers.length === 0 && (
              <tr>
                <td colSpan={6} className="text-center py-6 text-gray-500">
                  No users found.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
