"use client";

import { useEffect, useState } from "react";
import { apiDelete, apiGet } from "@/utils/api";
import { FaTrash, FaChevronUp, FaChevronDown } from "react-icons/fa";

interface ContactMessage {
  id: number;
  name: string;
  email: string;
  subject: string;
  message: string;
  sentAt: string;
}

export default function AdminFeedbackPage() {
  const [messages, setMessages] = useState<ContactMessage[]>([]);
  const [search, setSearch] = useState("");
  const [sortKey, setSortKey] = useState<keyof ContactMessage>("sentAt");
  const [sortAsc, setSortAsc] = useState(true);

  useEffect(() => {
    fetchMessages();
  }, []);

  const fetchMessages = async () => {
    const res = await apiGet<ContactMessage[]>("/api/contact/admin/all");
    setMessages(res);
  };

  const deleteMessage = async (id: number) => {
    if (!confirm("Are you sure you want to delete this message?")) return;
    try {
      await apiDelete(`/api/contact/admin/delete/${id}`);
      fetchMessages();
    } catch {
      alert("Failed to delete message.");
    }
  };

  const toggleSort = (key: keyof ContactMessage) => {
    if (key === sortKey) setSortAsc(!sortAsc);
    else {
      setSortKey(key);
      setSortAsc(true);
    }
  };

  const renderArrow = (key: keyof ContactMessage) => (
    <span className="inline-block w-4 ml-1">
      {sortKey === key && (sortAsc ? <FaChevronUp /> : <FaChevronDown />)}
    </span>
  );

  const filtered = messages
    .filter(
      (m) =>
        m.name.toLowerCase().includes(search.toLowerCase()) ||
        m.email.toLowerCase().includes(search.toLowerCase()) ||
        m.subject.toLowerCase().includes(search.toLowerCase()) ||
        m.message.toLowerCase().includes(search.toLowerCase())
    )
    .sort((a, b) => {
      const valA = a[sortKey];
      const valB = b[sortKey];
      return sortAsc
        ? `${valA}`.localeCompare(`${valB}`)
        : `${valB}`.localeCompare(`${valA}`);
    });

  return (
    <div className="min-h-screen bg-gray-100 p-6">
      <h1 className="text-3xl font-bold text-yellow-600 mb-6">
        📨 User Feedback
      </h1>

      <div className="mb-4 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-3">
        <div className="text-lg font-semibold">CONTACT MESSAGES</div>
        <input
          type="text"
          placeholder="Search..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="border px-3 py-2 rounded shadow-sm w-full sm:w-auto"
        />
      </div>

      <div className="hidden sm:block overflow-x-auto bg-white shadow rounded-lg">
        <table className="w-full text-sm text-left">
          <thead className="bg-gray-200">
            <tr>
              <th className="p-3">#</th>
              <th className="p-3 cursor-pointer" onClick={() => toggleSort("name")}>
                <span className="inline-flex items-center">
                  Name {renderArrow("name")}
                </span>
              </th>
              <th className="p-3 cursor-pointer" onClick={() => toggleSort("email")}>
                <span className="inline-flex items-center">
                  Email {renderArrow("email")}
                </span>
              </th>
              <th className="p-3 cursor-pointer" onClick={() => toggleSort("subject")}>
                <span className="inline-flex items-center">
                  Subject {renderArrow("subject")}
                </span>
              </th>
              <th className="p-3 cursor-pointer" onClick={() => toggleSort("message")}>
                <span className="inline-flex items-center">
                  Message {renderArrow("message")}
                </span>
              </th>
              <th className="p-3 cursor-pointer" onClick={() => toggleSort("sentAt")}>
                <span className="inline-flex items-center">
                  Date {renderArrow("sentAt")}
                </span>
              </th>
              <th className="p-3">Delete</th>
            </tr>
          </thead>
          <tbody>
            {filtered.map((msg, i) => (
              <tr key={msg.id} className="border-t">
                <td className="p-3">{i + 1}</td>
                <td className="p-3 max-w-xs h-24 overflow-hidden">
                  <div className="overflow-y-auto h-full whitespace-pre-wrap break-words pr-2">
                    {msg.name}
                  </div>
                </td>
                <td className="p-3 max-w-xs h-24 overflow-hidden">
                  <div className="overflow-y-auto h-full whitespace-pre-wrap break-words pr-2">
                    {msg.email}
                  </div>
                </td>
                <td className="p-3 max-w-xs h-24 overflow-hidden">
                  <div className="overflow-y-auto h-full whitespace-pre-wrap break-words pr-2">
                    {msg.subject}
                  </div>
                </td>
                <td className="p-3 max-w-xs h-24 overflow-hidden">
                  <div className="overflow-y-auto h-full whitespace-pre-wrap break-words pr-2">
                    {msg.message}
                  </div>
                </td>
                <td className="p-3">{new Date(msg.sentAt).toLocaleString()}</td>
                <td className="p-3">
                  <button
                    onClick={() => deleteMessage(msg.id)}
                    className="text-red-600 hover:text-red-800"
                  >
                    <FaTrash />
                  </button>
                </td>
              </tr>
            ))}
            {filtered.length === 0 && (
              <tr>
                <td colSpan={7} className="text-center py-6 text-gray-500">
                  No messages found.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      <div className="sm:hidden space-y-4">
        {filtered.length === 0 ? (
          <div className="text-center text-gray-500">No messages found.</div>
        ) : (
          filtered.map((msg, i) => (
            <div
              key={msg.id}
              className="border rounded-lg p-4 shadow bg-white space-y-2 text-sm"
            >
              <div>
                <strong>#{i + 1}</strong> - {new Date(msg.sentAt).toLocaleString()}
              </div>
              <div><strong>Name:</strong> {msg.name}</div>
              <div><strong>Email:</strong> {msg.email}</div>
              <div>
                <strong>Subject:</strong>
                <div className="whitespace-pre-wrap max-h-24 overflow-y-auto pr-1">
                  {msg.subject}
                </div>
              </div>
              <div>
                <strong>Message:</strong>
                <div className="whitespace-pre-wrap max-h-24 overflow-y-auto pr-1">
                  {msg.message}
                </div>
              </div>
              <button
                onClick={() => deleteMessage(msg.id)}
                className="mt-2 text-red-600 hover:text-red-800 flex items-center gap-1"
              >
                <FaTrash /> Delete
              </button>
            </div>
          ))
        )}
      </div>
    </div>
  );
}
