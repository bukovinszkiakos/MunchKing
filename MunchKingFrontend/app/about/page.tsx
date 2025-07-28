"use client";

import Image from "next/image";
import burgerImage from "@/public/burger2.png";
import { useRouter } from "next/navigation";
import { useAuth } from "../context/AuthContext";

export default function AboutPage() {
  const router = useRouter();
  const { user } = useAuth();

  const handleExploreClick = () => {
    router.push(user ? "/menu" : "/auth/login");
  };

  return (
    <div className="flex flex-col min-h-screen bg-[#1a1a1a] text-white">
      <main className="flex-1 flex items-center justify-center">
        <div className="max-w-6xl w-full px-4 py-10 flex flex-col lg:flex-row items-center gap-10">
          <div className="w-full lg:w-1/2 relative h-[420px]">
            <Image
              src={burgerImage}
              alt="Burger Illustration"
              fill
              className="object-contain"
              priority
              unoptimized
            />
          </div>

          <div className="w-full lg:w-1/2 text-center lg:text-left space-y-4">
            <h2 className="text-yellow-400 font-bold text-4xl lg:text-5xl font-serif">
              We Are MunchKing
            </h2>
            <p className="text-gray-300 text-lg leading-relaxed">
              Welcome to{" "}
              <span className="font-semibold text-white">MunchKing</span> – your
              go-to destination for mouthwatering fast food and lightning-fast
              delivery. We combine bold flavors, fresh ingredients, and
              unbeatable convenience to bring you meals that hit the spot every
              time.
            </p>
            <p className="text-gray-400 text-base">
              Whether you&apos;re grabbing a quick lunch, feeding the family, or
              enjoying a midnight snack &mdash; MunchKing is here to serve. Our chefs
              craft every burger, fry, and bite with passion and precision. Join
              thousands who trust MunchKing to satisfy their cravings.
            </p>
            <button
              onClick={handleExploreClick}
              className="bg-yellow-400 text-black px-6 py-3 rounded-full hover:bg-yellow-500 text-lg font-semibold mt-2"
            >
              Explore Our Menu
            </button>
          </div>
        </div>
      </main>

      <footer className="bg-[#111] text-gray-300 text-sm pt-12 pb-6">
        <div className="max-w-6xl mx-auto px-6 grid grid-cols-1 md:grid-cols-3 gap-10 text-left text-base">
          <div className="text-left">
            <h4 className="text-lg font-bold text-white mb-3">Contact Us</h4>
            <p className="mb-1">📍 Budapest, Hungary</p>
            <p className="mb-1">☎ +36 70 111 111</p>
            <p>✉ munchking@munchking.com</p>
          </div>

          <div className="text-center">
            <h4 className="text-lg font-bold text-white mb-3">MunchKing</h4>
            <p className="mb-3">
              We&apos;re passionate about crafting tasty fast food experiences using
              fresh ingredients and lightning-fast service.
            </p>
            <div className="flex justify-center gap-5 text-yellow-400 text-xl">
              <i className="fab fa-facebook-f"></i>
              <i className="fab fa-twitter"></i>
              <i className="fab fa-linkedin-in"></i>
              <i className="fab fa-instagram"></i>
              <i className="fab fa-pinterest"></i>
            </div>
          </div>

          <div className="text-right">
            <h4 className="text-lg font-bold text-white mb-3">Opening Hours</h4>
            <p className="mb-1">Everyday</p>
            <p>10.00 AM – 10.00 PM</p>
          </div>
        </div>

        <div className="text-center mt-10 text-sm text-gray-500">
          © {new Date().getFullYear()} MunchKing. All rights reserved.
        </div>
      </footer>
    </div>
  );
}
