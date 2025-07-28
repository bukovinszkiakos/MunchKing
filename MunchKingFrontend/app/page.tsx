"use client";
import Image from "next/image";
import burgerImg from "../public/burger.png";
import { useRouter } from "next/navigation";
import { useAuth } from "./context/AuthContext";

export default function HomePage() {
  const router = useRouter();
  const { user } = useAuth();

  const handleOrderNow = () => {
    if (user) {
      router.push("/menu");
    } else {
      router.push("/auth/login");
    }
  };

  return (
    <div className="relative min-h-screen bg-black text-white">
      <Image
        src={burgerImg}
        alt="Burger Background"
        fill
        className="object-cover z-0"
        priority
        unoptimized
      />

      <div className="absolute inset-0 bg-black bg-opacity-10 z-10 select-none" />

      <div className="relative z-20 flex flex-col min-h-screen ">
        <div className="flex-1 flex flex-col items-start justify-center px-10 lg:px-20 space-y-6 -mt-40">
          <h1 className="text-5xl lg:text-7xl font-bold font-serif select-none">
            Fast Food Restaurant
          </h1>
          <p className="text-gray-300 text-lg leading-relaxed max-w-xl select-none">
            Craving something tasty? MunchKing brings your favorite fast food straight to your door. 
            No lines, no waiting — just delicious meals delivered in no time.
          </p>
          <button
            onClick={handleOrderNow}
            className="bg-yellow-400 text-black px-6 py-3 rounded-full hover:bg-yellow-500 text-lg font-semibold w-full sm:w-fit select-none"
          >
            Order Now
          </button>
        </div>
      </div>
    </div>
  );
}
