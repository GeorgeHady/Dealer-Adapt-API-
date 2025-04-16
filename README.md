# Dealer Adapt API (Capstone Project - Mohawk College)

This project is the **backend API** for *Dealer Adapt*, a capstone software development project completed during my final (6th) semester at **Mohawk College** between **September and December 2024**.

Dealer Adapt API provides a comprehensive set of **RESTful endpoints** for managing automotive dealership operations, including inventory, customer records, and sales transactions. It is built using **ASP.NET Core** and is designed to integrate seamlessly with frontend clients and external services.

> 🔗 [Visit Project Homepage](https://rimush.co/)

> ⚠️ This repository only includes the **backend (ASP.NET Core API)**.  
> The **frontend**, developed separately using **React / Next.js**, is not included here.

---

## 🚀 Key Features

- **Inventory Management**  
  Add, update, and delete inventory items.

- **Customer Management**  
  Create, update, and delete customer records.

- **Sales Processing**  
  Manage sales transactions with full CRUD operations.

---

## 🛠️ Technologies Used

- **ASP.NET Core 8**
- **Entity Framework Core**
- **Identity Framework**
- **JWT (JSON Web Tokens)**
- **Swagger (OpenAPI)**
- **CORS (Cross-Origin Resource Sharing)**

---

## 📂 `Program.cs` Overview

### Service Registrations
- `Controllers`
- `Swagger` (API documentation)
- `DbContext` using SQL Server
- `Identity` for authentication & roles
- `Logging` services
- `EmailSender`, `UserService`, `JwtService`
- `RoleSeeder` for default roles
- `CORS` policy configuration
- `Authentication` (JWT)
- `Authorization`

### Middleware Pipeline
- Swagger UI (Development)
- Developer Exception Page (Development)
- Centralized Exception Handler (Production)
- HSTS & HTTPS Redirection
- Static Files
- Routing
- CORS Middleware
- JWT Authentication
- Authorization
- Custom `ApiKeyMiddleware`

---

## 🔐 Authentication

Authentication is handled via **JWT tokens**, with configuration defined in `appsettings.json`. Secure endpoints require valid tokens issued during login.

---

## ▶️ Getting Started

```bash
# 1. Clone the repository
git clone https://github.com/GeorgeHady/Dealer-Adapt-API-Backend.git

# 2. Navigate to the project folder
cd Dealer-Adapt-API-Backend

# 3. Configure your database connection
#    (Edit appsettings.json with your SQL Server connection string)

# 4. Run the application
dotnet run


Swagger UI will be available at: https://localhost:<port>/swagger


📄 License
This project is licensed under the MIT License.

