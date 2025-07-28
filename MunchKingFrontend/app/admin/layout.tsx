"use client";

import { ReactNode } from "react";
import AdminSidebarLayout from "../components/Sidebar/AdminSidebarLayout";

export default function AdminLayout({ children }: { children: ReactNode }) {
  return <AdminSidebarLayout>{children}</AdminSidebarLayout>;
}
