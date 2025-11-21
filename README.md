# Task Management API

A RESTful API for managing tasks with persistent storage.

## Features
- ✅ Create, Read, Update, Delete tasks
- ✅ SQL Server database with Entity Framework Core
- ✅ Async/await for optimal performance
- ✅ RESTful design with proper HTTP status codes
- ✅ Data persistence across restarts

## Tech Stack
- ASP.NET Core 8.0
- Entity Framework Core 8.0
- SQL Server LocalDB
- C# 12

## Database Schema
**Tasks Table:**
- Id (int, PK, auto-increment)
- Title (nvarchar, nullable)
- Description (nvarchar, nullable)
- IsCompleted (bit, default: false)
- CreatedAt (datetime2)

## API Endpoints
| Method | Endpoint | Description | Status Codes |
|--------|----------|-------------|--------------|
| GET | `/api/tasks` | Get all tasks | 200 |
| GET | `/api/tasks/{id}` | Get task by ID | 200, 404 |
| POST | `/api/tasks` | Create new task | 201 |
| PUT | `/api/tasks/{id}` | Update task | 200, 404 |
| DELETE | `/api/tasks/{id}` | Delete task | 204, 404 |

## Coming Soon
- JWT Authentication
- Task categories
- Search and filtering
- Input validation