"use client";

import { useEffect, useState } from "react";
import { apiDelete, apiGet } from "@/utils/api";
import { FaTrash } from "react-icons/fa";
import { FaArrowUp, FaArrowDown } from "react-icons/fa6";
import { useAuth } from "../../context/AuthContext"; 

interface User {
  id: string;
  fullName: string;
  username: string;
  email: string;
  createdAt: string;
  roles: string[];
}

export default function AdminUsersPage() {
  const { user: currentUser } = useAuth();
  const isSuperAdmin = currentUser?.isSuperAdmin;
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

  const toggleSort = (key: keyof User) => {
    if (key === sortKey) setSortAsc(!sortAsc);
    else {
      setSortKey(key);
      setSortAsc(true);
    }
  };

  const renderArrow = (key: keyof User) => {
    if (sortKey !== key) return <span className="inline-block w-4" />;
    return sortAsc ? (
      <FaArrowUp className="inline ml-1" />
    ) : (
      <FaArrowDown className="inline ml-1" />
    );
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
      return sortAsc
        ? `${aVal}`.localeCompare(`${bVal}`)
        : `${bVal}`.localeCompare(`${aVal}`);
    });

  const formatDate = (iso: string) =>
    new Date(iso).toLocaleString("hu-HU", {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
    });

  return (
    <div className="min-h-screen bg-gray-100 p-4 sm:p-6">
      <h1 className="text-2xl sm:text-3xl font-bold text-yellow-600 mb-4 sm:mb-6">
        👥 Manage Users
      </h1>

      <div className="mb-4 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-3">
        <div className="text-lg font-semibold">USERS LIST</div>
        <input
          type="text"
          placeholder="Search..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="border px-3 py-2 rounded shadow-sm w-full sm:w-auto"
        />
      </div>

      <div className="overflow-x-auto bg-white shadow rounded-lg">
        <div className="hidden sm:table w-full text-sm text-left">
          <div className="table-header-group bg-gray-200">
            <div className="table-row">
              <div className="table-cell p-3 font-semibold">#</div>
              <div
                className="table-cell p-3 font-semibold cursor-pointer"
                onClick={() => toggleSort("fullName")}
              >
                Full Name{renderArrow("fullName")}
              </div>
              <div
                className="table-cell p-3 font-semibold cursor-pointer"
                onClick={() => toggleSort("username")}
              >
                Username{renderArrow("username")}
              </div>
              <div
                className="table-cell p-3 font-semibold cursor-pointer"
                onClick={() => toggleSort("email")}
              >
                Email{renderArrow("email")}
              </div>
              <div
                className="table-cell p-3 font-semibold cursor-pointer"
                onClick={() => toggleSort("createdAt")}
              >
                Joined{renderArrow("createdAt")}
              </div>
              <div className="table-cell p-3 font-semibold">Delete</div>
            </div>
          </div>

          <div className="table-row-group">
            {sortedUsers.map((user, index) => {
              const isProtected =
                !isSuperAdmin && user.roles.includes("SuperAdmin");
              return (
                <div key={user.id} className="table-row border-t">
                  <div className="table-cell p-3">{index + 1}</div>
                  <div className="table-cell p-3">{user.fullName}</div>
                  <div className="table-cell p-3">{user.username}</div>
                  <div className="table-cell p-3">{user.email}</div>
                  <div className="table-cell p-3">
                    {formatDate(user.createdAt)}
                  </div>
                  <div className="table-cell p-3">
                    {isProtected ? (
                      <span className="text-gray-400 italic">Protected</span>
                    ) : (
                      <button
                        onClick={() => deleteUser(user.id)}
                        className="text-red-600 hover:text-red-800"
                      >
                        <FaTrash />
                      </button>
                    )}
                  </div>
                </div>
              );
            })}
          </div>
        </div>

        <div className="sm:hidden">
          {sortedUsers.map((user, index) => {
            const isProtected =
              !isSuperAdmin && user.roles.includes("SuperAdmin");
            return (
              <div key={user.id} className="border-b p-4">
                <p className="text-sm mb-1">
                  <strong>#{index + 1}</strong>
                </p>
                <p className="text-sm">
                  <strong>Name:</strong> {user.fullName}
                </p>
                <p className="text-sm">
                  <strong>Username:</strong> {user.username}
                </p>
                <p className="text-sm">
                  <strong>Email:</strong> {user.email}
                </p>
                <p className="text-sm">
                  <strong>Joined:</strong> {formatDate(user.createdAt)}
                </p>
                {isProtected ? (
                  <span className="text-gray-400 italic">Protected</span>
                ) : (
                  <button
                    onClick={() => deleteUser(user.id)}
                    className="mt-2 text-red-600 hover:text-red-800 text-sm flex items-center gap-1"
                  >
                    <FaTrash /> Delete
                  </button>
                )}
              </div>
            );
          })}
        </div>

        {sortedUsers.length === 0 && (
          <div className="text-center py-6 text-gray-500">No users found.</div>
        )}
      </div>
    </div>
  );
}
