"use client";

import Image from "next/image";
import { FaUser, FaShoppingCart } from "react-icons/fa";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { useAuth } from "../../context/AuthContext";
import { useCart } from "../../context/CartContext";
import burgerImage from "../../../public/burger.png";

export default function Navbar() {
  const pathname = usePathname();
  const { user, logout } = useAuth();
  const { items } = useCart();

  const totalQuantity = items.reduce((sum, item) => sum + item.quantity, 0);

  const linkClass = (path: string) =>
    `uppercase cursor-pointer hover:text-yellow-400 ${
      pathname === path ? "text-yellow-400" : "text-white"
    }`;

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

      <div className="relative z-10 flex justify-between items-center h-full px-10">
        <div className="font-bold text-3xl text-yellow-400 flex items-center gap-2">
          🍔 MunchKing
        </div>

        <div className="flex items-center gap-10 text-lg font-semibold">
          <Link href="/" className={linkClass("/")}>
            Home
          </Link>
          <Link href="/menu" className={linkClass("/menu")}>
            Menu
          </Link>
          <Link href="/about" className={linkClass("/about")}>
            About
          </Link>
          <Link href="/contact" className={linkClass("/contact")}>
            Contact
          </Link>
        </div>

        <div className="flex items-center gap-6">
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
            {totalQuantity > 0 && (
              <span className="absolute -top-2 -right-2 text-xs bg-yellow-400 text-black rounded-full px-2 font-bold">
                {totalQuantity}
              </span>
            )}
          </Link>

          {user ? (
            <button
              onClick={logout}
              className="bg-yellow-400 text-black text-lg px-5 py-2 rounded-full hover:bg-yellow-500 transition duration-200 font-semibold"
            >
              Logout
            </button>
          ) : (
            <Link href="/auth/login">
              <button className="bg-yellow-400 text-black text-lg px-5 py-2 rounded-full hover:bg-yellow-500 transition duration-200 font-semibold">
                Login
              </button>
            </Link>
          )}
        </div>
      </div>
    </div>
  );
}
