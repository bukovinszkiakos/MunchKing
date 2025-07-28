"use client";

import { useEffect, useRef, useState } from "react";
import { apiDelete, apiGet, apiPost, apiPut } from "../../../utils/api";
import { FaEdit, FaTrash } from "react-icons/fa";
import React from "react";
import Image from "next/image";

interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  isAvailable: boolean;
  categoryId: number;
  categoryName: string;
  createdAt?: string;
}

interface Category {
  id: number;
  name: string;
}

export default function AdminProductsPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [preview, setPreview] = useState<string | null>(null);
  const [expandedId, setExpandedId] = useState<number | null>(null);
  const [sortField, setSortField] = useState<keyof Product | null>(null);
  const [sortAsc, setSortAsc] = useState(true);
  const fileRef = useRef<HTMLInputElement>(null);

  const [form, setForm] = useState({
    id: 0,
    name: "",
    description: "",
    price: 0,
    image: null as File | null,
    categoryId: 0,
    isAvailable: true,
  });

  useEffect(() => {
    fetchProducts();
    fetchCategories();
  }, []);

  const fetchProducts = async () => {
    const data = await apiGet<Product[]>("/api/fooditems");
    setProducts(data);
  };

  const fetchCategories = async () => {
    const data = await apiGet<Category[]>("/api/categories");
    setCategories(data);
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const target = e.target;
    const { name, value, type } = target;
    const isCheckbox = type === "checkbox";

    setForm((prev) => ({
      ...prev,
      [name]: isCheckbox ? (target as HTMLInputElement).checked : type === "number" ? +value : value,
    }));
  };

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setForm((prev) => ({ ...prev, image: file }));
      setPreview(URL.createObjectURL(file));
    }
  };

  const uploadImage = async (file: File) => {
    const formData = new FormData();
    formData.append("image", file);
    const res = await apiPost<{ imageUrl: string }>("/api/fooditems/upload-image", formData);
    return res.imageUrl;
  };

  const handleSubmit = async (e: React.FormEvent) => {
  e.preventDefault();

  const formData = new FormData();
  formData.append("name", form.name);
  formData.append("description", form.description);
  formData.append("price", form.price.toString());
  formData.append("categoryId", form.categoryId.toString());
  formData.append("isAvailable", form.isAvailable.toString());

  if (form.image) {
    formData.append("image", form.image);
  }

  if (form.id === 0) {
    await apiPost("/api/fooditems", formData);
  } else {
    await apiPut(`/api/fooditems/${form.id}`, formData);
  }

  clearForm();
  fetchProducts();
};


  const clearForm = () => {
    setForm({
      id: 0,
      name: "",
      description: "",
      price: 0,
      image: null,
      categoryId: 0,
      isAvailable: true,
    });
    setPreview(null);
    if (fileRef.current) fileRef.current.value = "";
  };

  const handleEdit = (product: Product) => {
    setForm({
      id: product.id,
      name: product.name,
      description: product.description,
      price: product.price,
      image: null,
      categoryId: product.categoryId,
      isAvailable: product.isAvailable,
    });
    setPreview(product.imageUrl);
  };

  const handleDelete = async (id: number) => {
    if (confirm("Are you sure you want to delete this product?")) {
      await apiDelete(`/api/fooditems/${id}`);
      fetchProducts();
    }
  };

  const formatDate = (iso?: string) =>
    iso ? new Date(iso).toLocaleString("en-US", { dateStyle: "medium", timeStyle: "short" }) : "";

  const handleSort = (field: keyof Product) => {
    if (sortField === field) {
      setSortAsc(!sortAsc);
    } else {
      setSortField(field);
      setSortAsc(true);
    }
  };

  const renderArrow = (field: keyof Product) => {
    if (sortField !== field) return null;
    return sortAsc ? " ▲" : " ▼";
  };

  const sortedProducts = [...products].sort((a, b) => {
    if (!sortField) return 0;
    const aVal = a[sortField];
    const bVal = b[sortField];

    if (typeof aVal === "string" && typeof bVal === "string") {
      return sortAsc ? aVal.localeCompare(bVal) : bVal.localeCompare(aVal);
    }

    if (typeof aVal === "number" && typeof bVal === "number") {
      return sortAsc ? aVal - bVal : bVal - aVal;
    }

    if (typeof aVal === "boolean" && typeof bVal === "boolean") {
      return sortAsc ? Number(aVal) - Number(bVal) : Number(bVal) - Number(aVal);
    }

    return 0;
  });

  return (
    <div className="min-h-screen bg-gray-100 p-4 sm:p-6">
      <h1 className="text-2xl sm:text-3xl font-bold text-yellow-600 mb-6">
        🍔 Manage Products
      </h1>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="bg-white p-4 sm:p-6 rounded-lg shadow">
          <h2 className="text-lg sm:text-xl font-semibold mb-4">
            {form.id ? "Edit" : "Add New"} Product
          </h2>
          <form onSubmit={handleSubmit} className="space-y-4">
            <input name="name" value={form.name} onChange={handleChange} placeholder="Product name" required className="w-full border px-4 py-2 rounded text-sm" />
            <input name="description" value={form.description} onChange={handleChange} placeholder="Description" required className="w-full border px-4 py-2 rounded text-sm" />
            <input type="number" name="price" value={form.price} onChange={handleChange} placeholder="Price" required className="w-full border px-4 py-2 rounded text-sm" />
            <select name="categoryId" value={form.categoryId} onChange={handleChange} required className="w-full border px-4 py-2 rounded text-sm">
              <option value="">Select Category</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>
            <input type="file" accept="image/*" onChange={handleImageChange} ref={fileRef} className="w-full text-sm" />
            <label className="flex items-center gap-2 text-sm">
              <input type="checkbox" name="isAvailable" checked={form.isAvailable} onChange={handleChange} />
              Available
            </label>
            <div className="flex gap-3">
              <button type="submit" className="bg-blue-500 hover:bg-blue-600 text-white text-sm px-4 py-2 rounded">
                {form.id ? "Update" : "Add"}
              </button>
              <button type="button" onClick={clearForm} className="bg-gray-300 hover:bg-gray-400 text-sm px-4 py-2 rounded">Clear</button>
            </div>
            {preview && (
              <Image src={preview} alt="Preview" width={96} height={96} className="object-contain rounded border mt-2" unoptimized />
            )}
          </form>
        </div>

        <div className="lg:col-span-2 bg-white p-4 sm:p-6 rounded-lg shadow overflow-x-auto">
          <h2 className="text-lg sm:text-xl font-semibold mb-4">Product List</h2>
          <table className="w-full text-sm border-collapse">
            <thead>
              <tr className="bg-gray-200 text-left">
                <th className="p-2">#</th>
                <th className="p-2 cursor-pointer" onClick={() => handleSort("name")}>
                  Name{renderArrow("name")}
                </th>
                <th className="p-2 hidden sm:table-cell">Image</th>
                <th className="p-2 cursor-pointer" onClick={() => handleSort("price")}>
                  Price{renderArrow("price")}
                </th>
                <th className="p-2 hidden sm:table-cell">Category</th>
                <th className="p-2 cursor-pointer" onClick={() => handleSort("isAvailable")}>
                  Available{renderArrow("isAvailable")}
                </th>
                <th className="p-2 cursor-pointer" onClick={() => handleSort("createdAt")}>
                  Created{renderArrow("createdAt")}
                </th>
                <th className="p-2">Actions</th>
              </tr>
            </thead>
            <tbody>
              {sortedProducts.map((p) => (
                <React.Fragment key={p.id}>
                  <tr className="border-t">
                    <td className="p-2 text-lg font-bold cursor-pointer" onClick={() => setExpandedId(expandedId === p.id ? null : p.id)}>+</td>
                    <td className="p-2">{p.name}</td>
                    <td className="p-2 hidden sm:table-cell">
                      {p.imageUrl ? (
                        <Image src={p.imageUrl} alt={p.name} width={48} height={48} className="rounded" unoptimized />
                      ) : "-"}
                    </td>
                    <td className="p-2">${p.price.toFixed(2)}</td>
                    <td className="p-2 hidden sm:table-cell">{p.categoryName}</td>
                    <td className="p-2">
                      <span className={`text-xs font-semibold px-2 py-1 rounded-full ${p.isAvailable ? "bg-green-200 text-green-700" : "bg-red-200 text-red-700"}`}>
                        {p.isAvailable ? "Available" : "Out of stock"}
                      </span>
                    </td>
                    <td className="p-2">{formatDate(p.createdAt)}</td>
                    <td className="p-2 flex gap-2 text-sm">
                      <button onClick={() => handleEdit(p)} className="text-blue-600 hover:underline"><FaEdit /></button>
                      <button onClick={() => handleDelete(p.id)} className="text-red-600 hover:underline"><FaTrash /></button>
                    </td>
                  </tr>
                  {expandedId === p.id && (
                    <tr className="bg-gray-50">
                      <td colSpan={8} className="p-4 text-sm text-gray-700">
                        <p><strong>Description:</strong> {p.description}</p>
                        {p.createdAt && <p><strong>Created:</strong> {formatDate(p.createdAt)}</p>}
                      </td>
                    </tr>
                  )}
                </React.Fragment>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
