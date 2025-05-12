"use client";

import { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";

export default function RegisterPage() {
  const router = useRouter();

  const [form, setForm] = useState({
    fullName: "",
    username: "",
    email: "",
    mobileNumber: "",
    address: "",
    zipCode: "",
    password: "",
    profilePicture: null as File | null,
  });

  const [error, setError] = useState("");

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files?.[0]) {
      setForm((prev) => ({ ...prev, profilePicture: e.target.files![0] }));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    const formData = new FormData();
    formData.append("FullName", form.fullName);
    formData.append("Username", form.username);
    formData.append("Email", form.email);
    formData.append("MobileNumber", form.mobileNumber);
    formData.append("Address", form.address);
    formData.append("PostalCode", form.zipCode);
    formData.append("Password", form.password);
    if (form.profilePicture) {
      formData.append("ProfileImage", form.profilePicture);
    }

    try {
      const res = await fetch("/Auth/Register", {
        method: "POST",
        body: formData,
      });

      if (!res.ok) {
        const text = await res.text();
        throw new Error(text || "Registration failed");
      }

      router.push("/auth/login");
    } catch (err: any) {
      setError(err.message || "Something went wrong.");
    }
  };

  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center px-4">
      <div className="bg-white shadow-lg rounded-lg p-10 w-full max-w-4xl">
        <h2 className="text-3xl font-bold text-center text-yellow-500 mb-8">User Registration</h2>

        {error && <p className="text-red-500 text-center text-sm mb-4">{error}</p>}

        <form onSubmit={handleSubmit} className="grid grid-cols-1 md:grid-cols-2 gap-6">
          <input
            type="text"
            name="fullName"
            placeholder="Enter Full Name"
            value={form.fullName}
            onChange={handleChange}
            className="input-field"
            required
          />
          <input
            type="text"
            name="address"
            placeholder="Enter Address"
            value={form.address}
            onChange={handleChange}
            className="input-field"
            required
          />
          <input
            type="text"
            name="username"
            placeholder="Enter Username"
            value={form.username}
            onChange={handleChange}
            className="input-field"
            required
          />
          <input
            type="text"
            name="zipCode"
            placeholder="Enter Post/Zip Code"
            value={form.zipCode}
            onChange={handleChange}
            className="input-field"
            required
          />
          <input
            type="email"
            name="email"
            placeholder="Enter Email"
            value={form.email}
            onChange={handleChange}
            className="input-field"
            required
          />
          <input
            type="file"
            accept="image/*"
            onChange={handleFileChange}
            className="file-input"
          />
          <input
            type="text"
            name="mobileNumber"
            placeholder="Enter Mobile Number"
            value={form.mobileNumber}
            onChange={handleChange}
            className="input-field"
            required
          />
          <input
            type="password"
            name="password"
            placeholder="Enter Password"
            value={form.password}
            onChange={handleChange}
            className="input-field"
            required
          />

          <div className="col-span-full">
            <button
              type="submit"
              className="w-full bg-yellow-400 text-black font-semibold py-3 rounded-lg hover:bg-yellow-500 transition duration-200"
            >
              Register
            </button>
          </div>
        </form>

        <p className="text-center mt-6 text-sm">
          Already registered?{" "}
          <Link href="/auth/login" className="text-yellow-500 hover:underline">
            Login here
          </Link>
        </p>
      </div>
    </div>
  );
}
