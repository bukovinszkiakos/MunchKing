"use client";

import { useEffect, useRef, useState } from "react";
import { apiDelete, apiGet, apiPost, apiPut } from "../../../utils/api";
import { FaEdit, FaTrash } from "react-icons/fa";
import React from "react";

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

    let imageUrl =
      form.image !== null
        ? await uploadImage(form.image)
        : products.find((p) => p.id === form.id)?.imageUrl || "";

    const dto = {
      name: form.name,
      description: form.description,
      price: form.price,
      imageUrl,
      categoryId: form.categoryId,
      isAvailable: form.isAvailable,
    };

    if (form.id === 0) {
      await apiPost("/api/fooditems", dto);
    } else {
      await apiPut(`/api/fooditems/${form.id}`, dto);
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

  return (
    <div className="min-h-screen bg-gray-100 p-6">
      <h1 className="text-3xl font-bold text-yellow-600 mb-6">🍔 Manage Products</h1>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="bg-white p-6 rounded-lg shadow">
          <h2 className="text-xl font-semibold mb-4">{form.id ? "Edit" : "Add New"} Product</h2>
          <form onSubmit={handleSubmit} className="space-y-4">
            <label className="block text-sm font-semibold">Product Name</label>
            <input name="name" value={form.name} onChange={handleChange} required className="w-full border px-4 py-2 rounded" />

            <label className="block text-sm font-semibold">Product Description</label>
            <input name="description" value={form.description} onChange={handleChange} required className="w-full border px-4 py-2 rounded" />

            <label className="block text-sm font-semibold">Product Price ($)</label>
            <input type="number" name="price" value={form.price} onChange={handleChange} required className="w-full border px-4 py-2 rounded" />

            <label className="block text-sm font-semibold">Category</label>
            <select name="categoryId" value={form.categoryId} onChange={handleChange} required className="w-full border px-4 py-2 rounded">
              <option value="">Select Category</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>

            <label className="block text-sm font-semibold">Product Image</label>
            <input type="file" accept="image/*" onChange={handleImageChange} ref={fileRef} className="w-full" />

            <label className="flex items-center gap-2">
              <input type="checkbox" name="isAvailable" checked={form.isAvailable} onChange={handleChange} />
              Available
            </label>

            <div className="flex gap-3">
              <button type="submit" className="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded">
                {form.id ? "Update" : "Add"}
              </button>
              <button type="button" onClick={clearForm} className="bg-gray-300 hover:bg-gray-400 px-4 py-2 rounded">
                Clear
              </button>
            </div>

            {preview && <img src={preview} alt="Preview" className="h-24 object-contain rounded border mt-2" />}
          </form>
        </div>

        <div className="lg:col-span-2 bg-white p-6 rounded-lg shadow overflow-auto">
          <h2 className="text-xl font-semibold mb-4">Product List</h2>
          <table className="w-full text-sm border-collapse">
            <thead>
              <tr className="bg-gray-200 text-left">
                <th className="p-2">#</th>
                <th className="p-2">Name</th>
                <th className="p-2">Image</th>
                <th className="p-2">Price</th>
                <th className="p-2">Category</th>
                <th className="p-2">Available</th>
                <th className="p-2">Actions</th>
              </tr>
            </thead>
            <tbody>
              {products.map((p) => (
                <React.Fragment key={p.id}>
                  <tr className="border-t">
                    <td className="p-2 cursor-pointer text-xl font-bold" onClick={() => setExpandedId(expandedId === p.id ? null : p.id)}>+</td>
                    <td className="p-2">{p.name}</td>
                    <td className="p-2">{p.imageUrl ? <img src={p.imageUrl} alt={p.name} className="w-12 h-12 object-cover rounded" /> : "-"}</td>
                    <td className="p-2">${p.price.toFixed(2)}</td>
                    <td className="p-2">{p.categoryName}</td>
                    <td className="p-2">
                      <span className={`text-xs font-semibold px-2 py-1 rounded-full ${p.isAvailable ? "bg-green-200 text-green-700" : "bg-red-200 text-red-700"}`}>
                        {p.isAvailable ? "Available" : "Out of stock"}
                      </span>
                    </td>
                    <td className="p-2 flex gap-2">
                      <button onClick={() => handleEdit(p)} className="text-blue-600 hover:underline"><FaEdit /></button>
                      <button onClick={() => handleDelete(p.id)} className="text-red-600 hover:underline"><FaTrash /></button>
                    </td>
                  </tr>
                  {expandedId === p.id && (
                    <tr className="bg-gray-50">
                      <td colSpan={7} className="p-4 text-sm text-gray-700">
                        <p><strong>Description:</strong> {p.description}</p>
                        {p.createdAt && <p><strong>Created At:</strong> {formatDate(p.createdAt)}</p>}
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
