"use client";

import { useState } from "react";
import { useCart } from "../context/CartContext";
import { apiPost } from "@/utils/api";
import { useRouter } from "next/navigation";

export default function CheckoutPage() {
  const { items, total, clearCart } = useCart();
  const router = useRouter();

  const [paymentMode, setPaymentMode] = useState("CARD");
  const [cardOwner, setCardOwner] = useState("");
  const [cardNumber, setCardNumber] = useState("");
  const [expMonth, setExpMonth] = useState("");
  const [expYear, setExpYear] = useState("");
  const [cvv, setCvv] = useState("");
  const [address, setAddress] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const validateCard = () => {
    if (cardOwner.length < 2) return "Card owner name too short.";
    if (!/^\d{16}$/.test(cardNumber)) return "Card number must be 16 digits.";
    if (!/^\d{2}$/.test(expMonth) || +expMonth > 12 || +expMonth < 1) return "Invalid month.";
    if (!/^\d{4}$/.test(expYear)) return "Invalid year.";
    if (!/^\d{3}$/.test(cvv)) return "Invalid CVV.";
    return "";
  };

  const handleSubmit = async () => {
    setError("");
    if (!address) return setError("Delivery address is required.");

    if (paymentMode === "CARD") {
      const validationError = validateCard();
      if (validationError) return setError(validationError);
    }

    try {
      const response = await apiPost("/api/orders/checkout", {
        paymentMode,
        items: items.map((item) => ({
          foodItemId: item.foodItemId,
          quantity: item.quantity,
        })),
      });

      setSuccess("Order placed successfully!");
      clearCart();
      setTimeout(() => router.push("/profile"), 1500);
    } catch (err: any) {
      setError("Failed to place order. Please try again.");
    }
  };

  return (
    <div className="min-h-screen pt-28 px-4 flex justify-center bg-gray-100">
      <div className="bg-white rounded-xl p-10 shadow-xl max-w-xl w-full">
        <h1 className="text-2xl font-bold text-center text-yellow-500 mb-6">
          Order Payment
        </h1>

        <div className="flex gap-4 justify-center mb-6">
          <button
            className={`px-4 py-2 rounded-full font-semibold border flex items-center gap-2 ${
              paymentMode === "CARD"
                ? "bg-yellow-400 text-black border-yellow-400"
                : "bg-white border-gray-300"
            }`}
            onClick={() => setPaymentMode("CARD")}
          >
            💳 Credit Card
          </button>
          <button
            className={`px-4 py-2 rounded-full font-semibold border flex items-center gap-2 ${
              paymentMode === "CASH"
                ? "bg-yellow-400 text-black border-yellow-400"
                : "bg-white border-gray-300"
            }`}
            onClick={() => setPaymentMode("CASH")}
          >
            💸 Cash on Delivery
          </button>
        </div>

        {paymentMode === "CARD" && (
          <div className="grid grid-cols-1 gap-4 mb-4">
            <input
              type="text"
              placeholder="Card Owner"
              className="border p-2 rounded"
              value={cardOwner}
              onChange={(e) => setCardOwner(e.target.value)}
            />
            <input
              type="text"
              placeholder="Card Number (16 digits)"
              className="border p-2 rounded"
              value={cardNumber}
              onChange={(e) => setCardNumber(e.target.value)}
            />
            <div className="grid grid-cols-3 gap-4">
              <input
                type="text"
                placeholder="MM"
                className="border p-2 rounded"
                value={expMonth}
                onChange={(e) => setExpMonth(e.target.value)}
              />
              <input
                type="text"
                placeholder="YYYY"
                className="border p-2 rounded"
                value={expYear}
                onChange={(e) => setExpYear(e.target.value)}
              />
              <input
                type="text"
                placeholder="CVV"
                className="border p-2 rounded"
                value={cvv}
                onChange={(e) => setCvv(e.target.value)}
              />
            </div>
          </div>
        )}

        <textarea
          className="w-full border p-2 rounded mb-4"
          placeholder="Delivery Address"
          value={address}
          onChange={(e) => setAddress(e.target.value)}
        />

        <div className="flex justify-between items-center font-semibold text-lg mb-4">
          <span>Order Total:</span>
          <span className="text-green-600">${total.toFixed(2)}</span>
        </div>

        {error && <div className="text-red-500 text-sm mb-2">{error}</div>}
        {success && <div className="text-green-600 text-sm mb-2">{success}</div>}

        <button
          className="w-full bg-green-500 hover:bg-green-600 text-white font-bold py-3 rounded"
          onClick={handleSubmit}
        >
          Confirm Order
        </button>
      </div>
    </div>
  );
}