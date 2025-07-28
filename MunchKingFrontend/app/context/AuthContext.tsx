"use client";

import React, { createContext, useContext, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { apiGet, apiPost } from "@/utils/api";

interface User {
  id?: string;
  email?: string;
  username?: string;
  roles?: string[];
  isAdmin?: boolean;
  isSuperAdmin?: boolean;
  expirationUnix?: number;
}

interface AuthContextType {
  user: User | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  fetchUser: () => Promise<User | null>;
  loading: boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const router = useRouter();

  const fetchUser = async (): Promise<User | null> => {
    try {
      const res: Partial<User> = await apiGet("/Auth/Me");
      if (!res?.email) {
        setUser(null);
        return null;
      }

      const roles = res.roles || [];
      const userData: User = {
        ...res,
        roles,
        isAdmin: roles.includes("Admin") || roles.includes("SuperAdmin"),
        isSuperAdmin: roles.includes("SuperAdmin"),
      };

      setUser(userData);
      return userData;
    } catch {
      setUser(null);
      return null;
    } finally {
      setLoading(false);
    }
  };

  const login = async (email: string, password: string) => {
    await apiPost("/Auth/Login", { email, password });

    const userData = await fetchUser();
    if (!userData) return;

    router.refresh();

    setTimeout(() => {
      if (userData.isAdmin) {
        router.replace("/admin");
      } else {
        router.replace("/menu");
      }
    }, 50);
  };

  const logout = async () => {
    await apiPost("/Auth/Logout", {});
    setUser(null);
    router.refresh();
    router.replace("/auth/login");
  };

  useEffect(() => {
    fetchUser();
  }, []);

  return (
    <AuthContext.Provider value={{ user, login, logout, fetchUser, loading }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
};
