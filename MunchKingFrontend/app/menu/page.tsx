"use client";

import { useEffect, useState } from "react";
import Image from "next/image";
import { useRouter } from "next/navigation";
import { apiGet } from "@/utils/api";
import { useCart } from "../context/CartContext";
import { useAuth } from "../context/AuthContext";

type Category = { id: number; name: string };
type FoodItem = {
  id: number;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  isAvailable: boolean;
  categoryId: number;
  categoryName: string;
  categoryIsActive: boolean;
};

export default function MenuPage() {
  const { user, loading } = useAuth();
  const router = useRouter();
  const [categories, setCategories] = useState<Category[]>([]);
  const [foodItems, setFoodItems] = useState<FoodItem[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<number | null>(null);
  const [success, setSuccess] = useState("");

  const { addToCart } = useCart();

  useEffect(() => {
    if (!loading && !user) {
      router.replace("/auth/login");
    }
  }, [loading, user]);

  useEffect(() => {
    if (user) {
      apiGet<Category[]>("/api/categories").then(setCategories);
      apiGet<FoodItem[]>("/api/fooditems").then(setFoodItems);
    }
  }, [user]);

  if (loading || !user) {
    return (
      <div className="min-h-screen pt-24 flex justify-center items-center text-xl">
        Loading...
      </div>
    );
  }

  const filteredItems = selectedCategory
    ? foodItems.filter((f) => f.categoryId === selectedCategory)
    : foodItems;

  return (
    <div className="min-h-screen pt-24 px-6 md:px-16 bg-gray-100">
      <h1 className="text-4xl font-bold text-center text-yellow-500 mb-6">Our Menu</h1>

      {success && (
        <div className="mb-6 text-center bg-green-100 text-green-800 px-4 py-2 rounded font-semibold shadow">
          {success}
        </div>
      )}

      <div className="flex justify-center flex-wrap gap-4 mb-10">
        <button
          onClick={() => setSelectedCategory(null)}
          className={`px-4 py-2 rounded-full font-semibold ${
            selectedCategory === null
              ? "bg-yellow-400 text-black"
              : "bg-white text-gray-700"
          }`}
        >
          All
        </button>
        {categories.map((cat) => (
          <button
            key={cat.id}
            onClick={() => setSelectedCategory(cat.id)}
            className={`px-4 py-2 rounded-full font-semibold ${
              selectedCategory === cat.id
                ? "bg-yellow-400 text-black"
                : "bg-white text-gray-700"
            }`}
          >
            {cat.name}
          </button>
        ))}
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8 pb-20">
        {filteredItems.map((item) => {
          const disabled = !item.isAvailable || !item.categoryIsActive;
          return (
            <div
              key={item.id}
              className={`bg-white shadow-md rounded-lg overflow-hidden relative ${
                disabled ? "opacity-60" : ""
              }`}
            >
              <div className="w-full h-48 bg-gray-200 relative">
                {item.imageUrl && (
                  <Image
                    src={item.imageUrl}
                    alt={item.name}
                    fill
                    className="object-cover"
                    unoptimized
                  />
                )}
              </div>
              <div className="p-4">
                <h2 className="text-xl font-semibold text-yellow-500">{item.name}</h2>
                <p className="text-gray-600 mt-1">{item.description}</p>
                <div className="mt-2 space-y-1">
                  {!item.isAvailable && (
                    <span className="inline-block bg-red-200 text-red-700 text-xs font-bold px-2 py-1 rounded">
                      Out of Stock
                    </span>
                  )}
                  {!item.categoryIsActive && (
                    <span className="inline-block bg-gray-200 text-gray-700 text-xs font-bold px-2 py-1 rounded">
                      Inactive Category
                    </span>
                  )}
                </div>
                <div className="mt-4 flex justify-between items-center">
                  <span className="font-bold text-lg">${item.price.toFixed(2)}</span>
                  <button
                    onClick={() => {
                      if (disabled) return;
                      addToCart({
                        foodItemId: item.id,
                        name: item.name,
                        price: item.price,
                        imageUrl: item.imageUrl,
                        quantity: 1,
                      });
                      setSuccess(`${item.name} added to cart!`);
                      setTimeout(() => setSuccess(""), 3000);
                    }}
                    disabled={disabled}
                    className={`px-4 py-2 rounded-full text-sm font-semibold transition ${
                      disabled
                        ? "bg-gray-300 text-gray-500 cursor-not-allowed"
                        : "bg-yellow-400 text-black hover:bg-yellow-500"
                    }`}
                  >
                    {disabled ? "Unavailable" : "Add to Cart"}
                  </button>
                </div>
              </div>
            </div>
          );
        })}
      </div>
    </div>
  );
}
