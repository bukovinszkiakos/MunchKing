"use client";

import { useEffect, useRef, useState } from "react";
import { FaEdit, FaTrash } from "react-icons/fa";
import { apiDelete, apiGet, apiPost, apiPut } from "../../../utils/api";
import Image from "next/image";

interface Category {
  id: number;
  name: string;
  imageUrl: string;
  isActive: boolean;
  createdAt: string;
}

export default function AdminCategoriesPage() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [form, setForm] = useState({
    name: "",
    image: null as File | null,
    isActive: true,
  });
  const [editingId, setEditingId] = useState<number | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const [sortField, setSortField] = useState<keyof Category | null>(null);
  const [sortAsc, setSortAsc] = useState(true);

  const fetchCategories = async () => {
    const result = await apiGet<Category[]>("/api/categories");
    setCategories(result);
  };

  useEffect(() => {
    fetchCategories();
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, type, checked } = e.target;
    setForm((prev) => ({
      ...prev,
      [name]: type === "checkbox" ? checked : value,
    }));
  };

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setForm((prev) => ({ ...prev, image: file }));
      setPreviewUrl(URL.createObjectURL(file));
    }
  };

  const clearForm = () => {
    setForm({ name: "", image: null, isActive: true });
    setEditingId(null);
    setPreviewUrl(null);
    if (fileInputRef.current) fileInputRef.current.value = "";
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const formData = new FormData();
    formData.append("name", form.name);
    formData.append("isActive", String(form.isActive));
    if (form.image) formData.append("image", form.image);

    if (editingId) {
      await apiPut(`/api/categories/${editingId}`, formData);
    } else {
      await apiPost("/api/categories", formData);
    }

    clearForm();
    await fetchCategories();
  };

  const handleEdit = (cat: Category) => {
    setEditingId(cat.id);
    setForm({
      name: cat.name,
      image: null,
      isActive: cat.isActive,
    });
    setPreviewUrl(null);
    if (fileInputRef.current) fileInputRef.current.value = "";
  };

  const handleDelete = async (id: number) => {
    if (confirm("Are you sure you want to delete this category?")) {
      await apiDelete(`/api/categories/${id}`);
      await fetchCategories();
    }
  };

  const formatDate = (iso: string) =>
    new Date(iso).toLocaleString("hu-HU", {
      year: "numeric",
      month: "2-digit",
      day: "2-digit",
      hour: "2-digit",
      minute: "2-digit",
    });

  const handleSort = (field: keyof Category) => {
    if (sortField === field) {
      setSortAsc(!sortAsc);
    } else {
      setSortField(field);
      setSortAsc(true);
    }
  };

  const renderArrow = (field: keyof Category) => {
    if (sortField !== field) return null;
    return sortAsc ? " ▲" : " ▼";
  };

  const sortedCategories = [...categories].sort((a, b) => {
    if (!sortField) return 0;

    const aValue = a[sortField];
    const bValue = b[sortField];

    if (typeof aValue === "string" && typeof bValue === "string") {
      return sortAsc
        ? aValue.localeCompare(bValue)
        : bValue.localeCompare(aValue);
    }

    if (typeof aValue === "boolean" && typeof bValue === "boolean") {
      return sortAsc
        ? Number(aValue) - Number(bValue)
        : Number(bValue) - Number(aValue);
    }

    return 0;
  });

  return (
    <div className="min-h-screen bg-gray-100 p-4 sm:p-6">
      <h1 className="text-2xl sm:text-3xl font-bold text-yellow-600 mb-6">
        🍽️ Manage Categories
      </h1>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="bg-white p-4 sm:p-6 rounded-lg shadow-md">
          <h2 className="text-lg sm:text-xl font-semibold mb-4">
            {editingId ? "Edit Category" : "Add New Category"}
          </h2>
          <form onSubmit={handleSubmit} className="space-y-4">
            <input
              type="text"
              name="name"
              value={form.name}
              onChange={handleChange}
              placeholder="Enter category name"
              required
              className="w-full border px-4 py-2 rounded-lg"
            />
            <input
              type="file"
              accept="image/*"
              ref={fileInputRef}
              onChange={handleImageChange}
              className="w-full"
            />
            <label className="flex items-center gap-2">
              <input
                type="checkbox"
                name="isActive"
                checked={form.isActive}
                onChange={handleChange}
              />
              Active
            </label>
            <div className="flex flex-col sm:flex-row gap-4">
              <button
                type="submit"
                className="bg-blue-500 hover:bg-blue-600 text-white font-semibold px-4 py-2 rounded-lg w-full sm:w-auto"
              >
                {editingId ? "Update" : "Add"}
              </button>
              <button
                type="button"
                onClick={clearForm}
                className="bg-gray-300 hover:bg-gray-400 px-4 py-2 rounded-lg w-full sm:w-auto"
              >
                Clear
              </button>
            </div>

            {previewUrl && (
              <div className="mt-4">
                <p className="text-sm text-gray-600 mb-2">Preview:</p>
                <Image
                  src={previewUrl}
                  alt="Preview"
                  width={96}
                  height={96}
                  className="object-cover rounded shadow"
                  unoptimized
                />
              </div>
            )}
          </form>
        </div>

        <div className="lg:col-span-2 bg-white p-4 sm:p-6 rounded-lg shadow-md overflow-auto">
          <h2 className="text-lg sm:text-xl font-semibold mb-4">Category List</h2>
          <div className="w-full overflow-x-auto">
            <table className="w-full text-sm border-collapse">
              <thead>
                <tr className="bg-gray-200 text-left">
                  <th
                    className="p-2 whitespace-nowrap cursor-pointer"
                    onClick={() => handleSort("name")}
                  >
                    Name{renderArrow("name")}
                  </th>
                  <th className="p-2 whitespace-nowrap">Image</th>
                  <th
                    className="p-2 whitespace-nowrap cursor-pointer"
                    onClick={() => handleSort("isActive")}
                  >
                    Status{renderArrow("isActive")}
                  </th>
                  <th
                    className="p-2 whitespace-nowrap cursor-pointer"
                    onClick={() => handleSort("createdAt")}
                  >
                    Created{renderArrow("createdAt")}
                  </th>
                  <th className="p-2 whitespace-nowrap">Actions</th>
                </tr>
              </thead>
              <tbody>
                {sortedCategories.map((cat) => (
                  <tr key={cat.id} className="border-t">
                    <td className="p-2">{cat.name}</td>
                    <td className="p-2">
                      <Image
                        src={cat.imageUrl}
                        alt={cat.name}
                        width={48}
                        height={48}
                        className="object-cover rounded"
                        unoptimized
                      />
                    </td>
                    <td className="p-2">
                      <span
                        className={`text-xs font-semibold px-2 py-1 rounded-full ${
                          cat.isActive
                            ? "bg-green-200 text-green-700"
                            : "bg-red-200 text-red-700"
                        }`}
                      >
                        {cat.isActive ? "Active" : "Inactive"}
                      </span>
                    </td>
                    <td className="p-2">{formatDate(cat.createdAt)}</td>
                    <td className="p-2 flex gap-2">
                      <button
                        onClick={() => handleEdit(cat)}
                        className="text-blue-600 hover:text-blue-800"
                      >
                        <FaEdit />
                      </button>
                      <button
                        onClick={() => handleDelete(cat.id)}
                        className="text-red-600 hover:text-red-800"
                      >
                        <FaTrash />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
}
