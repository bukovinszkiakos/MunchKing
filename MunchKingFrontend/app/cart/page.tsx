"use client";

import { useCart } from "../context/CartContext";
import Image from "next/image";
import Link from "next/link";
import { useAuth } from "../context/AuthContext";

export default function CartPage() {
  const { items, updateQuantity, removeFromCart, total } = useCart();
  const { user, loading } = useAuth();

 

  if (loading) return <p className="mt-32 text-center">Loading...</p>;
  if (!user) {
    if (typeof window !== "undefined") {
      window.location.href = "/auth/login";
    }
    return null;
  }


  return (
    <div className="min-h-screen pt-24 px-4 sm:px-6 md:px-16 bg-gray-100">
      <h1 className="text-3xl font-bold mb-6 text-yellow-500 text-center">
        🛒 Your Shopping Cart
      </h1>

      {items.length === 0 ? (
        <p className="text-center text-gray-600">Your cart is empty.</p>
      ) : (
        <div className="bg-white shadow rounded-lg p-4 sm:p-6">
          <div className="md:hidden space-y-4">
            {items.map((item) => (
              <div key={item.foodItemId} className="border rounded p-4 flex flex-col gap-3">
                <div className="flex items-center gap-4">
                  <Image
                    src={item.imageUrl}
                    alt={item.name}
                    width={64}
                    height={64}
                    className="object-cover rounded"
                    unoptimized
                  />
                  <div>
                    <p className="font-bold">{item.name}</p>
                    <p className="text-sm text-gray-600">
                      ${item.price.toFixed(2)} each
                    </p>
                  </div>
                </div>

                <div className="flex justify-between items-center">
                  <label className="text-sm">Qty:</label>
                  <input
                    type="number"
                    min="1"
                    value={item.quantity}
                    onChange={(e) =>
                      updateQuantity(item.foodItemId, parseInt(e.target.value))
                    }
                    className="w-20 border rounded px-2 py-1 text-center"
                  />
                </div>

                <div className="flex justify-between items-center">
                  <p className="font-semibold">
                    Total: ${(item.price * item.quantity).toFixed(2)}
                  </p>
                  <button
                    onClick={() => {
                      if (confirm("Do you want to remove this item from cart?")) {
                        removeFromCart(item.foodItemId);
                      }
                    }}
                    className="text-red-500 hover:text-red-700 font-bold text-xl"
                  >
                    ×
                  </button>
                </div>
              </div>
            ))}
          </div>

          <div className="hidden md:block">
            <table className="w-full text-sm border-collapse">
              <thead className="bg-gray-100 text-left">
                <tr>
                  <th className="p-3">Image</th>
                  <th className="p-3">Product</th>
                  <th className="p-3">Unit Price</th>
                  <th className="p-3">Quantity</th>
                  <th className="p-3">Total</th>
                  <th className="p-3">Remove</th>
                </tr>
              </thead>
              <tbody>
                {items.map((item) => (
                  <tr key={item.foodItemId} className="border-t">
                    <td className="p-3">
                      <Image
                        src={item.imageUrl}
                        alt={item.name}
                        width={64}
                        height={64}
                        className="object-cover rounded"
                        unoptimized
                      />
                    </td>
                    <td className="p-3 font-semibold">{item.name}</td>
                    <td className="p-3">${item.price.toFixed(2)}</td>
                    <td className="p-3">
                      <input
                        type="number"
                        min="1"
                        value={item.quantity}
                        onChange={(e) =>
                          updateQuantity(item.foodItemId, parseInt(e.target.value))
                        }
                        className="w-16 border rounded px-2 py-1"
                      />
                    </td>
                    <td className="p-3 font-semibold">
                      ${(item.price * item.quantity).toFixed(2)}
                    </td>
                    <td className="p-3">
                      <button
                        onClick={() => {
                          if (confirm("Do you want to remove this item from cart?")) {
                            removeFromCart(item.foodItemId);
                          }
                        }}
                        className="text-red-500 hover:text-red-700 font-bold text-xl"
                      >
                        ×
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="flex flex-col md:flex-row justify-between items-center mt-6 gap-4">
            <Link href="/menu" className="text-blue-600 hover:underline text-sm md:text-base">
              ← Continue Shopping
            </Link>
            <div className="text-right">
              <p className="text-lg font-semibold mb-2">
                Grand Total: ${total.toFixed(2)}
              </p>
              <Link
                href="/checkout"
                className="inline-block bg-green-500 hover:bg-green-600 text-white px-6 py-2 rounded font-semibold text-center"
              >
                Checkout →
              </Link>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
