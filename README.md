# 🍽️ MunchKing

## 📌 About The Project

**MunchKing – Full-Featured Online Food Ordering Platform**

MunchKing is a complete online food ordering system that lets users browse a categorized menu, add items to their cart, and place orders with a preferred payment method. The platform features an admin dashboard for managing food items, categories, users, feedback, and orders.

Built with a modern **Next.js frontend** and a robust **ASP.NET Core Web API** backend, MunchKing supports role-based access, real-time order management, and a responsive UI for both customers and admins.

---

### ✨ Core Features

* ✅ User registration & login with JWT
* ✅ Dynamic menu categorized by food type
* ✅ Product images, descriptions, prices, and stock availability
* ✅ Add to cart, edit cart, and checkout
* ✅ Choose payment method (Card or Cash on Delivery)
* ✅ Order history and profile management
* ✅ Admin dashboard to manage food, categories, orders, and users
* ✅ Role-based access: User vs Admin vs SuperAdmin
* ✅ Contact form & admin feedback panel
* ✅ Daily/weekly/monthly sales summaries
* ✅ Mobile responsive design

---

## 🖼️ Live Demo (Screenshots)

> Below are screenshots showcasing **some** of the core pages and features of the MunchKing platform.  
> These images highlight selected parts of the app and do not represent the full feature set.

### 🏠 Home Page

<img src="https://github.com/user-attachments/assets/b98c2feb-4b17-436a-bf58-cee7254c6f3a" width="75%" />

### ℹ️ About Page

<img src="https://github.com/user-attachments/assets/9dcd36e0-185b-4389-b209-f8fa23de0942" width="75%" />

### 📬 Contact Page

<img src="https://github.com/user-attachments/assets/aeacaaa1-d468-4a18-979d-47210b0ec7df" width="75%" />

### 🛒 Cart Page

<img src="https://github.com/user-attachments/assets/7e84d29b-671e-435c-84c4-b216d1609348" width="75%" />

### 💳 Credit Card Payment

<img src="https://github.com/user-attachments/assets/5cdecb62-c6bc-4558-bf13-115e52a02af8" width="75%" />

### 🍔 Menu Page (User View)

<img src="https://github.com/user-attachments/assets/8b8f8211-bab4-40f0-a7a7-1a19eae9af5a" width="75%" />

### 🧑‍🍳 Admin Dashboard Overview

<img src="https://github.com/user-attachments/assets/49957b60-6628-4f22-8134-9f097d846909" width="75%" />

### 🗂️ Manage Categories (Admin Panel)

<img src="https://github.com/user-attachments/assets/1a26cae7-4595-429c-8fa5-1cf948d41f39" width="75%" />

---

## 🔧 Built With

- [![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-5C2D91?style=for-the-badge\&logo=dotnet\&logoColor=white)](https://dotnet.microsoft.com/en-us/)
- [![Next.js](https://img.shields.io/badge/Next.js-000000?style=for-the-badge\&logo=nextdotjs\&logoColor=white)](https://nextjs.org/)
- [![React](https://img.shields.io/badge/React-20232A?style=for-the-badge\&logo=react\&logoColor=61DAFB)](https://reactjs.org/)
- [![Tailwind CSS](https://img.shields.io/badge/Tailwind_CSS-38B2AC?style=for-the-badge\&logo=tailwind-css\&logoColor=white)](https://tailwindcss.com/)
- [![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge\&logo=jsonwebtokens\&logoColor=white)](https://jwt.io/)
- [![Entity Framework Core](https://img.shields.io/badge/Entity_Framework_Core-512BD4?style=for-the-badge\&logo=dotnet\&logoColor=white)](https://learn.microsoft.com/en-us/ef/)
- [![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge\&logo=microsoftsqlserver\&logoColor=white)](https://www.microsoft.com/en-us/sql-server)
- [![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge\&logo=docker\&logoColor=white)](https://www.docker.com/)

---

## 🚀 Getting Started

### 1️⃣ Prerequisites

Make sure you have the following installed:

* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [Node.js (v18+)](https://nodejs.org/)
* [Docker Desktop](https://www.docker.com/products/docker-desktop)

---

### 2️⃣ Clone the repository

```bash
git clone https://github.com/your-username/munchking.git
cd munchking
```

---

### 3️⃣ Run with Docker (Optional)

```bash
docker compose up -d
```

> This will start:
>
> * SQL Server
> * ASP.NET Core Web API
> * Next.js frontend

---

## 🔐 Authentication & Roles

MunchKing uses **JWT authentication** and supports three roles:

* 👤 **User** – Can browse menu, manage cart, and place orders
* 🛠️ **Admin** – Full access to product management, order processing, and dashboard
* 👑 **SuperAdmin** – Has all admin permissions + can promote/demote users and manage roles

---

## 🛕️ Ordering System

* Browse available food items by category
* Add/remove items from cart
* Choose payment method (Card or COD)
* Submit order and track status (Pending → In Progress → Completed)
* View purchase history in profile page

---

## 🛠️ Admin Features

* 🔧 Manage categories and food items (CRUD)
* 🛆 Track and update order statuses
* 📊 View sales reports (daily, weekly, monthly)
* 👥 View and manage users
* 💬 See user feedback via the contact system
* 📱 Fully responsive admin interface

---

## 🧪 Testing

Includes **unit and integration tests** for backend logic and API endpoints.

---

### ✅ Unit Tests

```bash
dotnet test MunchKing.UnitTests/MunchKing.UnitTests.csproj
```

Covers:

* OrderService
* AuthService
* Cart logic
* Role-based access
* DTO mapping

---

### ✅ Integration Tests

```bash
dotnet test MunchKing.IntegrationTests/MunchKing.IntegrationTests.csproj
```

Covers:

* Authentication flows
* Order processing
* Cart and checkout APIs
* Admin endpoints
* Full in-memory DB testing with JWT

---

## 🗓️ Roadmap

### ✅ Completed

* [x] JWT auth & role management
* [x] Cart and checkout flow
* [x] Admin dashboard
* [x] Product/category CRUD
* [x] Order status tracking
* [x] User feedback/contact system
* [x] Responsive layout
* [x] Docker support

### ⚠️ Planned

* [ ] Email confirmations
* [ ] Graphs and analytics for admins
* [ ] Invoice generation for completed orders

---

## 👨‍💻 Developer

* **Ákos Bukovinszki**
  [GitHub Profile](https://github.com/bukovinszkiakos)

---

## 🛡️ License

This project is licensed under the MIT License.

---

## 📬 Contact

📂 Repository: [https://github.com/bukovinszkiakos/MunchKing](https://github.com/bukovinszkiakos/MunchKing)

---

<p align="right">(<a href="#top">Back to top</a>)</p>
