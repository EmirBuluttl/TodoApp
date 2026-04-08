# 🚀 N-Tier Todo App

A complete, multi-user To-Do List application built with a modern **.NET 9 N-Tier Architecture**. The project is designed with SOLID principles, Repository & Unit of Work patterns, and features a completely isolated RESTful Web API along with a beautifully styled Vanilla JS frontend.

## ✨ Features

- **N-Tier Architecture**: Clean separation of concerns (Core, Data, Business, WebAPI).
- **Security**: JWT (JSON Web Token) authentication and BCrypt password hashing.
- **Data Isolation**: Users can securely fetch, create, edit, and delete only their own task items.
- **Entity Framework Core**: Code-first approach with SQLite and automatic migrations on startup.
- **Glassmorphism UI**: A lightweight, pure HTML/CSS/JS frontend included directly in the `wwwroot`.
- **Inline Editing**: Live, seamless editing of tasks directly from the frontend UI without page reloads.

## 🛠️ Technology Stack

- **Backend**: C#, .NET 9.0, ASP.NET Core Web API
- **Database**: SQLite, Entity Framework Core
- **Authentication**: JWT Bearer Authentication, BCrypt.Net-Next
- **Frontend**: Vanilla Javascript, HTML5, Vanilla CSS
- **Documentation**: Swagger UI

## 📁 Project Structure

```text
TodoApp/
│
├── TodoApp.Core/         # Domain models, Repository & Unit of Work Interfaces
├── TodoApp.Data/         # EF Core AppDbContext, Dapper/EF Repository implementations
├── TodoApp.Business/     # Business logic layer, Auth & Todo Services, DTOs
└── TodoApp.API/          # API Controllers, Dependency Injection, UI (wwwroot)
```

## 🚀 Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Installation & Run

1. Clone the repository:
```bash
git clone https://github.com/EmirBuluttl/TodoApp.git
cd TodoApp
```

2. Run the application (this will automatically apply Entity Framework migrations and create the local SQLite database):
```bash
dotnet run --project TodoApp.API
```

3. Open your browser and navigate to the UI:
```text
http://localhost:5074/
```
*(Or explore the interactive API documentation at http://localhost:5074/swagger)*

## 📡 Key API Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| `POST` | `/api/Auth/register`| Register a new user account | ❌ |
| `POST` | `/api/Auth/login`| Login and receive JWT token | ❌ |
| `GET`  | `/api/Todo` | Get all tasks for the logged in user | ✅ |
| `POST` | `/api/Todo` | Add a new task | ✅ |
| `PUT`  | `/api/Todo/{id}` | Update task title, details, or completion status | ✅ |
| `DELETE` | `/api/Todo/{id}` | Delete a task | ✅ |

---
*Built with ❤️ focusing on clean architecture.*
