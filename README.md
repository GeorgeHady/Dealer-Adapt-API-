https://rimush.co/

# Dealer Adapt API

The Dealer Adapt API provides a comprehensive set of RESTful endpoints to manage dealer operations, including inventory management. This API is designed to help dealer seamlessly integrate their systems with external services, ensuring efficient and streamlined operations. 



## Key Features
- **Inventory Management:** Add, update, and delete inventory items.
- **Customer Management:** Add, update, and delete customer records.
- **Sales Processing:** Create, update, and delete sales transactions.

## Technologies Used
- **ASP.NET Core**
- **Entity Framework Core**
- **Identity Framework**
- **Swagger**
- **JWT (JSON Web Tokens)**
- **CORS (Cross-Origin Resource Sharing)**

## Program.cs Overview

### Services Configuration
- **Controllers**
- **Swagger**
- **DbContext** with SQL Server
- **Identity**
- **Logging**
- **EmailSender**
- **UserService**
- **JwtService**
- **RoleSeeder**
- **CORS**
- **Authentication** with JWT
- **Authorization**

### Middleware Pipeline
- **Swagger** (development)
- **Developer Exception Page** (development)
- **Exception Handler** (production)
- **HSTS** (production)
- **HTTPS Redirection**
- **Static Files**
- **Routing**
- **CORS**
- **Authentication**
- **Authorization**
- **ApiKeyMiddleware**

### Authentication
The API uses JWT (JSON Web Tokens) for secure authentication, configured in `appsettings.json`.

## Getting Started
1. **Clone the repository:** `git clone <repository-url>`
2. **Navigate to the project directory:** `cd Dealer-Adapt-API`
3. **Set up the database connection:** Update the connection string in `appsettings.json`.
4. **Run the application:** `dotnet run`



## License
This project is licensed under the [MIT License](LICENSE).

For detailed documentation, refer to the API's official documentation.

