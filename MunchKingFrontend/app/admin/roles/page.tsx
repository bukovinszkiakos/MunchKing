"use client";

import { useEffect, useState } from "react";
import { useAuth } from "../../context/AuthContext";
import { useRouter } from "next/navigation";
import { apiGet, apiPut } from "@/utils/api";

interface User {
  id: string;
  username: string;
  email: string;
  isAdmin: boolean;
}

export default function ManageRolesPage() {
  const { user } = useAuth();
  const router = useRouter();
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState("");

  useEffect(() => {
  if (user === null) return; 

  if (!user?.isSuperAdmin) {
    router.replace("/");
    return;
  }

  fetchUsers();
}, [user]);


  const fetchUsers = async () => {
    try {
      const res = await apiGet<User[]>("/api/admin/users");
      setUsers(res);
    } catch {
      setMessage("Failed to load users.");
    }
  };

  const toggleAdmin = async (userId: string) => {
    try {
      setLoading(true);
      await apiPut(`/api/admin/users/${userId}/toggle-admin`, {});
      setMessage("Role updated.");
      fetchUsers();
    } catch {
      setMessage("Failed to update role.");
    } finally {
      setLoading(false);
      setTimeout(() => setMessage(""), 3000);
    }
  };

  return (
    <div className="min-h-screen bg-gray-100 pt-24 px-6">
      <h1 className="text-3xl font-bold text-yellow-500 mb-6 text-center">🛡️ Manage User Roles</h1>

      {message && (
        <div className="bg-green-100 text-green-800 px-4 py-2 mb-6 rounded shadow text-center font-semibold">
          {message}
        </div>
      )}

      <div className="bg-white rounded-xl shadow p-6 max-w-4xl mx-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="text-left border-b">
              <th className="py-2">Username</th>
              <th className="py-2">Email</th>
              <th className="py-2">Admin Role</th>
              <th className="py-2">Action</th>
            </tr>
          </thead>
          <tbody>
            {users.map((u) => (
              <tr key={u.id} className="border-b">
                <td className="py-2">{u.username}</td>
                <td className="py-2">{u.email}</td>
                <td className="py-2">{u.isAdmin ? "✅ Yes" : "❌ No"}</td>
                <td className="py-2">
                  <button
                    onClick={() => toggleAdmin(u.id)}
                    disabled={loading}
                    className={`px-3 py-1 rounded text-sm font-semibold ${
                      u.isAdmin
                        ? "bg-red-100 text-red-700 hover:bg-red-200"
                        : "bg-green-100 text-green-700 hover:bg-green-200"
                    }`}
                  >
                    {u.isAdmin ? "Remove Admin" : "Make Admin"}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
