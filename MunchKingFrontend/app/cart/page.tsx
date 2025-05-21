"use client";

import { useCart } from "../context/CartContext";
import Image from "next/image";
import Link from "next/link";

export default function CartPage() {
  const { items, updateQuantity, removeFromCart, total } = useCart();

  return (
    <div className="min-h-screen pt-24 px-6 md:px-16 bg-gray-100">
      <h1 className="text-3xl font-bold mb-6 text-yellow-500 text-center">🛒 Your Shopping Cart</h1>

      {items.length === 0 ? (
        <p className="text-center text-gray-600">Your cart is empty.</p>
      ) : (
        <div className="bg-white shadow rounded-lg p-6">
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
                    <img src={item.imageUrl} alt={item.name} className="w-16 h-16 object-cover rounded" />
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
                  <td className="p-3 font-semibold">${(item.price * item.quantity).toFixed(2)}</td>
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

          <div className="flex justify-between items-center mt-6">
            <Link href="/menu" className="text-blue-600 hover:underline">
              ← Continue Shopping
            </Link>
            <div className="text-right">
              <p className="text-lg font-semibold mb-2">Grand Total: ${total.toFixed(2)}</p>
              <Link
                href="/checkout"
                className="inline-block bg-green-500 hover:bg-green-600 text-white px-6 py-2 rounded font-semibold"
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
