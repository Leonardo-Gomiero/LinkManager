# 🔗 LinkManager API

LinkManager is a RESTful API built with .NET 9 that serves as a backend for a "Link in Bio" application (similar to Linktree).

This project demonstrates a modern, production-ready implementation of Minimal APIs, utilizing Clean Architecture principles, JWT Authentication, and Containerization.

## 🚀 Tech Stack

| Component | Technology |
|-----------|-----------|
| Framework | .NET 9 (Minimal APIs) |
| Language | C# 12 |
| Database | PostgreSQL |
| ORM | Entity Framework Core (Code-First) |
| Authentication | JWT (JSON Web Tokens) |
| Security | BCrypt (Password Hashing) |
| Containerization | Docker & Docker Compose |
| Architecture | Service Pattern with DTOs |

## 🏗️ Architecture Overview

The project follows a modular structure to ensure separation of concerns:

- **Endpoints** (`Program.cs`): Handles HTTP requests and response mapping
- **Services**: Contains business logic (validations, password hashing, token generation)
- **Data / Entities**: Database context and domain models
- **DTOs**: Data Transfer Objects to sanitize input/output and protect sensitive data
- **Extensions**: Helper methods (e.g., extracting User ID from Claims)

## 🐳 How to Run

This project supports two execution modes: Demo Mode (Full Docker) and Development Mode (Hybrid).

### Option 1: Demo Mode (Recommended for Testing)

Run the entire stack (API + Database) inside Docker containers. No .NET SDK required on the host machine.

```bash
# Build and start the containers
docker compose up --build

# API will be available at:
# http://localhost:8080
```

### Option 2: Development Mode (Hybrid)

Run the Database in Docker and the API locally (for hot-reloading and debugging).

**Prerequisites**: .NET 9 SDK

```bash
# Start only the Database container
docker compose -f docker-compose.dev.yml up -d

# Run the API locally
cd LinkManager.api
dotnet watch run

# API will be available at:
# http://localhost:5035 (or the port shown in your terminal)
```

### Swagger (Development)

The interactive Swagger documentation is available when running the API locally in development mode. Access the link below (replace the port if necessary):

[Abrir Swagger UI — http://localhost:5035/swagger/index.html](http://localhost:5035/swagger/index.html)

## 🔑 Authentication Flow

This API is secured with JWT:

1. Register a new user
2. Login to receive a Bearer Token
3. Send the token in the `Authorization` header for protected routes (`/links`)

## 📡 API Endpoints

### 👤 Users & Authentication

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/users` | Register a new user account | ❌ No |
| `GET` | `/users/{id}` | Get public user details | ❌ No |
| `PUT` | `/users/{id}` | Update user email, slug, or password | ❌ No |
| `POST` | `/login` | Authenticate and retrieve JWT Token | ❌ No |

### 🔗 Link Management

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/links` | Create a new link | ✅ Yes |
| `GET` | `/links` | List all links for the logged user | ✅ Yes |
| `PUT` | `/links/{id}` | Update a specific link | ✅ Yes |
| `DELETE` | `/links/{id}` | Delete a specific link | ✅ Yes |

## 🧪 Testing with cURL

### 1. Create User

```bash
curl -X POST http://localhost:8080/users \
  -H "Content-Type: application/json" \
  -d '{"email": "dev@example.com", "password": "securePass123", "pageSlug": "myprofile"}'
```

### 2. Login (Get Token)

```bash
curl -X POST http://localhost:8080/login \
  -H "Content-Type: application/json" \
  -d '{"email": "dev@example.com", "password": "securePass123"}'
```

Copy the token from the response.

### 3. Create Link (Protected)

```bash
curl -X POST http://localhost:8080/links \
  -H "Authorization: Bearer YOUR_TOKEN_HERE" \
  -H "Content-Type: application/json" \
  -d '{"title": "My Portfolio", "url": "https://github.com/myprofile"}'
```

## 📝 License

This project is open-source and available under the MIT License.