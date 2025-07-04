# 🚗 DriveSphere Car Rental Platform

Welcome to **DriveSphere**, a full-stack car rental management system designed to streamline car bookings, rental tracking, user management, and administrative control. This repository contains the **ASP.NET Core Web API backend** (`CarRentalHub.API`) that powers the platform.

---

## 🔧 Tech Stack

- **Backend:** ASP.NET Core Web API
- **Database:** SQL Server
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Core Identity
- **Architecture:** MVC, Repository Pattern
- **Tools:** Swagger (API Testing), Visual Studio / VS Code

---

## 📁 Project Structure (CarRentalHub.API)



---

## ✅ Features

- 🔐 User Registration and Login
- 👥 Role-based Access (Admin, User)
- 🚘 Car CRUD Operations (Admin)
- 📅 Car Booking & Availability Check
- 📊 Admin Dashboard to View Bookings
- 📖 Booking History for Users
- 🔍 Car Filtering and Searching
- 📌 Secure Authentication & Session Handling

---

## 🚀 Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) or VS Code

### Setup Instructions

1. **Clone the repository**

```bash
git clone https://github.com/SURAJPATIL6088/DriveSphere-Car-Rental-Platform.git
cd DriveSphere-Car-Rental-Platform/CarRentalHub.API
```

2. Update Database Connection

In appsettings.json, configure your SQL Server connection string:
```
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=CarRentalHubDB;Trusted_Connection=True;"
}
```

3. Apply EF Core Migrations

```
dotnet ef database update
```

4. Build the Application
```
dotnet clean
dotnet build
```

5. Run the Application
```
dotnet run
```




