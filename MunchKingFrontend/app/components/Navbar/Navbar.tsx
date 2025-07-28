"use client";

import Image from "next/image";
import { FaUser, FaShoppingCart, FaBars, FaTimes } from "react-icons/fa";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { useAuth } from "../../context/AuthContext";
import { useCart } from "../../context/CartContext";
import { useState, useEffect } from "react";
import burgerImage from "../../../public/burger.png";

export default function Navbar() {
  const pathname = usePathname();
  const { user, loading, logout, fetchUser } = useAuth();
  const { items } = useCart();
  const [isOpen, setIsOpen] = useState(false);

 useEffect(() => {
  if (!user && !loading) {
    fetchUser(); 
  }
}, [user, loading]);

if (loading) return null;

  const totalQuantity = items.reduce((sum, item) => sum + item.quantity, 0);

  const linkClass = (path: string) =>
    `uppercase block md:inline-block hover:text-yellow-400 ${
      pathname === path ? "text-yellow-400" : "text-white"
    }`;

  const toggleMenu = () => setIsOpen(!isOpen);

  return (
    <div className="fixed top-0 left-0 w-full h-20 z-50 text-white text-xl bg-black">
      <div className="absolute inset-0 pointer-events-none z-0">
        <Image
          src={burgerImage}
          alt="Burger Top"
          fill
          className="object-cover opacity-10"
          style={{ objectPosition: "top" }}
          unoptimized
        />
      </div>

      <div className="relative z-10 flex justify-between items-center h-full px-4 sm:px-6 lg:px-10">
        <div className="font-bold text-2xl sm:text-3xl text-yellow-400 flex items-center gap-2">
          🍔 MunchKing
        </div>

        <button
          className="md:hidden text-white text-2xl"
          onClick={toggleMenu}
          aria-label="Toggle Menu"
        >
          {isOpen ? <FaTimes /> : <FaBars />}
        </button>

        <div className="hidden md:flex items-center gap-8 text-lg font-semibold">
          <Link href="/" className={linkClass("/")}>Home</Link>
          <Link href="/menu" className={linkClass("/menu")}>Menu</Link>
          <Link href="/about" className={linkClass("/about")}>About</Link>
          <Link href="/contact" className={linkClass("/contact")}>Contact</Link>
        </div>

        <div className="hidden md:flex items-center gap-6">
          <Link href="/profile">
            <FaUser
              className={`text-xl cursor-pointer hover:text-yellow-400 ${
                pathname === "/profile" ? "text-yellow-400" : "text-white"
              }`}
            />
          </Link>

          <Link href="/cart" className="relative">
            <FaShoppingCart
              className={`text-xl cursor-pointer hover:text-yellow-400 ${
                pathname === "/cart" ? "text-yellow-400" : "text-white"
              }`}
            />
            {user && totalQuantity > 0 && (
              <span className="absolute -top-2 -right-2 text-xs bg-yellow-400 text-black rounded-full px-2 font-bold">
                {totalQuantity}
              </span>
            )}
          </Link>

          {user ? (
            <button
              onClick={logout}
              className="bg-yellow-400 text-black px-5 py-2 rounded-full hover:bg-yellow-500 transition text-sm font-semibold"
            >
              Logout
            </button>
          ) : (
            <Link href="/auth/login">
              <button className="bg-yellow-400 text-black px-5 py-2 rounded-full hover:bg-yellow-500 transition text-sm font-semibold">
                Login
              </button>
            </Link>
          )}
        </div>
      </div>

      {isOpen && (
        <div className="md:hidden bg-black bg-opacity-95 text-white px-6 py-4 flex flex-col gap-4 text-lg font-semibold absolute top-20 left-0 w-full z-40">
          <Link href="/" className={linkClass("/")} onClick={toggleMenu}>Home</Link>
          <Link href="/menu" className={linkClass("/menu")} onClick={toggleMenu}>Menu</Link>
          <Link href="/about" className={linkClass("/about")} onClick={toggleMenu}>About</Link>
          <Link href="/contact" className={linkClass("/contact")} onClick={toggleMenu}>Contact</Link>
          <Link href="/profile" onClick={toggleMenu}>
            <span className="flex items-center gap-2">
              <FaUser /> Profile
            </span>
          </Link>
          <Link href="/cart" onClick={toggleMenu}>
            <span className="flex items-center gap-2 relative">
              <FaShoppingCart />
              Cart
              {user && totalQuantity > 0 && (
                <span className="absolute -top-2 -right-3 text-xs bg-yellow-400 text-black rounded-full px-2 font-bold">
                  {totalQuantity}
                </span>
              )}
            </span>
          </Link>
          {user ? (
            <button
              onClick={() => {
                toggleMenu();
                logout();
              }}
              className="bg-yellow-400 text-black px-4 py-2 rounded-full hover:bg-yellow-500 transition text-sm font-semibold"
            >
              Logout
            </button>
          ) : (
            <Link href="/auth/login" onClick={toggleMenu}>
              <button className="bg-yellow-400 text-black px-4 py-2 rounded-full hover:bg-yellow-500 transition text-sm font-semibold w-full">
                Login
              </button>
            </Link>
          )}
        </div>
      )}
    </div>
  );
}
