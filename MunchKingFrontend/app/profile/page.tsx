"use client";

import { useEffect, useState } from "react";
import Image from "next/image";
import { useAuth } from "../context/AuthContext";
import { apiGet, apiPost } from "@/utils/api";
import PurchaseHistory from "../components/PurchaseHistory/PurchaseHistoryTab";

interface Profile {
  fullName: string;
  username: string;
  email: string;
  mobileNumber: string;
  address: string;
  postalCode: string;
  profileImageUrl?: string;
}

export default function ProfilePage() {
  const { fetchUser } = useAuth();
  const [profile, setProfile] = useState<Profile | null>(null);
  const [editing, setEditing] = useState(false);
  const [image, setImage] = useState<File | null>(null);
  const [refreshToken, setRefreshToken] = useState<number>(Date.now());
  const [activeTab, setActiveTab] = useState<"info" | "history">("info");

  useEffect(() => {
    async function loadProfile() {
      const res = await apiGet<Profile>("/api/profile");
      setProfile(res);
    }
    loadProfile();
  }, [refreshToken]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setProfile((prev) => prev && { ...prev, [name]: value });
  };

  const handleSave = async () => {
    if (!profile) return;

    const formData = new FormData();
    formData.append("fullName", profile.fullName);
    formData.append("username", profile.username);
    formData.append("email", profile.email);
    formData.append("mobileNumber", profile.mobileNumber);
    formData.append("address", profile.address);
    formData.append("postalCode", profile.postalCode);
    if (image) formData.append("profileImage", image);

    try {
      await apiPost("/api/profile/update", formData);
      await fetchUser();
      setEditing(false);
      setRefreshToken(Date.now());
    } catch (err) {
      console.error("Failed to update profile", err);
    }
  };

  if (!profile) return <p className="mt-32 text-center">Loading...</p>;

  return (
    <div className="max-w-5xl mx-auto mt-28 bg-white p-10 rounded-xl shadow-2xl">
      <div className="flex justify-between items-center mb-6">
        <div className="flex gap-2">
          <button
            onClick={() => setActiveTab("info")}
            className={`px-4 py-2 font-semibold rounded-t-lg border-b-2 ${
              activeTab === "info"
                ? "border-yellow-500 text-yellow-500"
                : "border-transparent text-gray-400"
            }`}
          >
            Basic Info
          </button>
          <button
            onClick={() => setActiveTab("history")}
            className={`px-4 py-2 font-semibold rounded-t-lg border-b-2 ${
              activeTab === "history"
                ? "border-yellow-500 text-yellow-500"
                : "border-transparent text-gray-400"
            }`}
          >
            Purchased History
          </button>
        </div>
      </div>

      {activeTab === "info" ? (
        <div>
          <div className="flex flex-col items-center">
            <div className="relative w-28 h-28 mb-4 border-4 border-yellow-400 rounded-full overflow-hidden">
              <img
                src={
                  profile.profileImageUrl
                    ? `${profile.profileImageUrl}?v=${refreshToken}`
                    : "/default_profile.png"
                }
                alt="Profile"
                className="w-full h-full rounded-full object-cover"
              />
            </div>
            {editing && (
              <input
                type="file"
                accept="image/*"
                onChange={(e) => setImage(e.target.files?.[0] || null)}
                className="mb-4"
              />
            )}
            <h2 className="text-2xl font-bold">{profile.fullName}</h2>
            <p className="text-gray-600">@{profile.username}</p>
            <p className="text-gray-500">{profile.email}</p>
          </div>

          <div className="mt-10 grid grid-cols-1 md:grid-cols-2 gap-6">
            <Input label="Full Name" name="fullName" value={profile.fullName} onChange={handleChange} disabled={!editing} />
            <Input label="Username" name="username" value={profile.username} onChange={handleChange} disabled={!editing} />
            <Input label="Email" name="email" value={profile.email} onChange={handleChange} disabled={!editing} />
            <Input label="Mobile Number" name="mobileNumber" value={profile.mobileNumber} onChange={handleChange} disabled={!editing} />
            <Input label="Address" name="address" value={profile.address} onChange={handleChange} disabled={!editing} />
            <Input label="Postal Code" name="postalCode" value={profile.postalCode} onChange={handleChange} disabled={!editing} />
          </div>

          <div className="mt-10 flex justify-end gap-4">
            {editing ? (
              <>
                <button className="px-4 py-2 bg-gray-300 hover:bg-gray-400 rounded" onClick={() => setEditing(false)}>
                  Cancel
                </button>
                <button className="px-4 py-2 bg-yellow-400 hover:bg-yellow-500 font-semibold rounded" onClick={handleSave}>
                  Save Changes
                </button>
              </>
            ) : (
              <button className="px-4 py-2 bg-yellow-400 hover:bg-yellow-500 font-semibold rounded" onClick={() => setEditing(true)}>
                Edit Details
              </button>
            )}
          </div>
        </div>
      ) : (
        <PurchaseHistory />
      )}
    </div>
  );
}

function Input({
  label,
  name,
  value,
  onChange,
  disabled,
}: {
  label: string;
  name: string;
  value: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  disabled: boolean;
}) {
  return (
    <div>
      <label className="block font-medium mb-1">{label}</label>
      <input
        type="text"
        name={name}
        value={value}
        onChange={onChange}
        disabled={disabled}
        className="w-full border px-3 py-2 rounded focus:outline-none focus:ring-2 focus:ring-yellow-400"
      />
    </div>
  );
}
