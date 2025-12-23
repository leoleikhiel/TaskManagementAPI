# 📋 Task Management API

A professional-grade RESTful API built with ASP.NET Core 8.0, featuring JWT authentication, service layer architecture, and advanced task management capabilities.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Entity Framework](https://img.shields.io/badge/EF%20Core-8.0-512BD4)](https://docs.microsoft.com/ef/core)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019+-CC2927?logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

> **Backend API for the Task Management System** | [UI Repository](https://github.com/leoleikhiel/task-management-ui)

---

## 🌟 Overview

This API provides a complete backend solution for task management with calendar integration, note-taking, and user authentication. Built with clean architecture principles and production-ready patterns.

**Key Highlights:**
- 🔐 JWT authentication with BCrypt password hashing
- 📅 Advanced calendar and scheduling features
- 📝 Task notes with time-based editing rules
- 📊 Analytics and statistics dashboard
- 🏗️ Clean service layer architecture
- 🔍 Optimized database queries with indexing

---

## 🚀 Features

### Core Functionality
- **User Authentication** - Register, login with JWT tokens (2-hour expiration)
- **Task Management** - Full CRUD operations with validation
- **Categories** - Organize tasks with custom categories
- **Search & Filter** - Multi-criteria filtering (status, priority, category, text search)
- **Analytics** - Task statistics, completion rates, category breakdowns

### Calendar & Scheduling
- 📅 **Task Scheduling** - Set ScheduledDate (work date) and DueDate (deadline)
- 📆 **Multiple Views** - Today, this week, overdue, custom date ranges
- 🗓️ **Calendar Grouping** - Tasks organized by date with smart display logic
- ⏰ **Overdue Detection** - Automatic calculation with priority sorting
- ✅ **Completion Tracking** - Hybrid auto/manual CompletedAt timestamps

### Task Notes
- 📝 **Rich Notes** - Add detailed notes to any task
- ⏱️ **1-Hour Edit Window** - Business rule enforcement for note editing
- 🗑️ **Cascade Delete** - Notes automatically removed with parent task
- 📜 **Note History** - View all notes with timestamps

---

## 🏗️ Architecture

### Design Patterns
```
┌─────────────┐
│ Controllers │ ← HTTP Layer
└──────┬──────┘
       │
┌──────▼──────┐
│  Services   │ ← Business Logic
└──────┬──────┘
       │
┌──────▼──────┐
│ Repository  │ ← Data Access (EF Core)
└──────┬──────┘
       │
┌──────▼──────┐
│  Database   │ ← SQL Server
└─────────────┘
```

**Patterns Implemented:**
- Service Layer Pattern
- Repository Pattern (via EF Core)
- Dependency Injection
- DTO Pattern

### Project Structure
```
TaskManagementAPI/
├── Controllers/        # HTTP endpoints
├── Services/          # Business logic interfaces & implementations
├── Models/            # Database entities
├── DTOs/              # Data transfer objects
├── Data/              # DbContext and migrations
└── Program.cs         # Configuration and DI setup
```

---

## 🛠️ Tech Stack

| Technology | Purpose |
|------------|---------|
| ASP.NET Core 8.0 | Web API framework |
| Entity Framework Core | ORM for database operations |
| SQL Server | Relational database |
| JWT Bearer | Token-based authentication |
| BCrypt.Net | Password hashing |
| Swagger/OpenAPI | API documentation |

---

## 📊 Database Schema

### Core Tables

**Users**
```sql
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY,
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    UserRole INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    INDEX IX_Users_Email (Email)
);
```

**Tasks**
```sql
CREATE TABLE Tasks (
    Id INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    IsCompleted BIT DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    DueDate DATETIME2,
    ScheduledDate DATETIME2,
    CompletedAt DATETIME2,
    UserId INT NOT NULL,
    CategoryId INT,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    INDEX IX_Tasks_UserId (UserId),
    INDEX IX_Tasks_DueDate (DueDate)
);
```

**TaskNotes**
```sql
CREATE TABLE TaskNotes (
    Id INT PRIMARY KEY IDENTITY,
    Content NVARCHAR(2000) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    TaskId INT NOT NULL,
    FOREIGN KEY (TaskId) REFERENCES Tasks(Id) ON DELETE CASCADE,
    INDEX IX_TaskNotes_TaskId_CreatedAt (TaskId, CreatedAt)
);
```

### Relationships
- **User → Tasks** (One-to-Many, CASCADE delete)
- **User → Categories** (One-to-Many, CASCADE delete)
- **Task → Notes** (One-to-Many, CASCADE delete)
- **Category → Tasks** (One-to-Many, SET NULL on delete)

---

## 📌 API Endpoints

### Authentication
```http
POST /api/auth/register        # Create new account
POST /api/auth/login           # Login and get JWT token
```

### Tasks
```http
GET    /api/tasks                    # Get all user tasks
GET    /api/tasks/{id}               # Get specific task
POST   /api/tasks                    # Create new task
PUT    /api/tasks/{id}               # Update task
DELETE /api/tasks/{id}               # Delete task
GET    /api/tasks/search             # Search tasks by title
GET    /api/tasks/filter             # Filter by status/category/priority
GET    /api/tasks/statistics         # Get analytics dashboard
PUT    /api/tasks/complete-all       # Mark all tasks complete
```

### Calendar & Scheduling
```http
GET /api/tasks/today              # Today's tasks
GET /api/tasks/week               # This week's tasks (Mon-Sun)
GET /api/tasks/overdue            # Overdue tasks (sorted by urgency)
GET /api/tasks/calendar           # Date range view (max 90 days)
GET /api/tasks/calendar/month     # Month-based calendar helper
```

### Task Notes
```http
GET    /api/tasks/{taskId}/notes           # Get all notes for task
GET    /api/tasks/{taskId}/notes/{noteId}  # Get specific note
POST   /api/tasks/{taskId}/notes           # Add note to task
PUT    /api/tasks/{taskId}/notes/{noteId}  # Update note (1-hour window)
DELETE /api/tasks/{taskId}/notes/{noteId}  # Delete note
```

### Categories
```http
GET    /api/categories        # Get all user categories
POST   /api/categories        # Create category
PUT    /api/categories/{id}   # Update category
DELETE /api/categories/{id}   # Delete category
```

**Total:** 24 REST endpoints

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server) or LocalDB
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/leoleikhiel/TaskManagementAPI.git
cd TaskManagementAPI
```

2. **Configure database connection**

Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "your-super-secret-key-minimum-32-characters-long-for-production",
    "Issuer": "TaskManagementAPI",
    "Audience": "TaskManagementClient"
  }
}
```

3. **Apply database migrations**
```bash
dotnet ef database update
```

4. **Run the application**
```bash
dotnet run
```

5. **Access Swagger UI**
```
https://localhost:7001/swagger
```

### Test Data Seeder

The API includes a comprehensive test data seeder that runs automatically in development:

**Test Accounts:**
```
Regular User:
Email: john.doe@example.com
Password: Password123!

Admin User:
Email: jane.smith@example.com
Password: Password456!
```

**Includes:**
- 2 users with different roles
- 8 categories
- 22 tasks (today, week, month, overdue, completed)
- Multiple notes per task

---

## 🔐 Security Features

- ✅ **BCrypt Password Hashing** - Industry-standard with automatic salting
- ✅ **JWT Tokens** - HS256 signing, 2-hour expiration
- ✅ **Data Isolation** - Users can only access their own data
- ✅ **Authorization Middleware** - All endpoints require authentication
- ✅ **Input Validation** - Comprehensive validation with error messages
- ✅ **SQL Injection Protection** - Parameterized queries via EF Core

---

## 📈 Performance Optimizations

### Database Indexing
```csharp
// Optimized for common queries
IX_Users_Email              // O(1) authentication lookups
IX_Tasks_UserId             // O(log n) user task filtering
IX_Tasks_DueDate            // O(log n) overdue/calendar queries
IX_TaskNotes_TaskId_CreatedAt  // O(log n) note retrieval + sorting
```

### Time Complexity Analysis
| Operation | Complexity | Notes |
|-----------|------------|-------|
| Get user tasks | O(log n) | Indexed on UserId |
| Create task | O(1) | Direct insertion |
| Update task | O(1) | Primary key lookup |
| Search tasks | O(n) | Linear text search |
| Calendar grouping | O(m) | m = filtered tasks |
| Overdue sorted | O(k log k) | k = overdue tasks |

---

## 🧪 Testing

### Using Swagger UI
1. Navigate to `https://localhost:7001/swagger`
2. Click **Authorize** button
3. Register a new user via `/api/auth/register`
4. Login via `/api/auth/login` and copy the JWT token
5. Paste token in the Authorize dialog: `Bearer {your-token}`
6. Test any endpoint

### Using Postman
1. Import the API collection (if provided)
2. Set environment variable: `baseUrl = https://localhost:7001`
3. Login to get JWT token
4. Add token to Authorization header: `Bearer {token}`

---

## 🐛 Common Issues & Solutions

### Issue: "Cannot connect to database"
```bash
# Update connection string in appsettings.json
# Ensure SQL Server is running
# Verify you have the correct server name
```

### Issue: "JWT token validation failed"
```bash
# Ensure Jwt:Key in appsettings.json is at least 32 characters
# Check token hasn't expired (2-hour limit)
# Verify token format: "Bearer {token}"
```

### Issue: "Migration failed"
```bash
# Delete existing database
# Run: dotnet ef database drop
# Then: dotnet ef database update
```

---

## 📚 API Response Examples

### Successful Login
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe"
  }
}
```

### Calendar View Response
```json
{
  "2025-11-20": [
    {
      "id": 1,
      "title": "Team Meeting",
      "scheduledDate": "2025-11-20T10:00:00",
      "dueDate": "2025-11-20T11:00:00",
      "isCompleted": false
    }
  ],
  "2025-11-21": [
    {
      "id": 2,
      "title": "Project Review",
      "scheduledDate": "2025-11-21T14:00:00",
      "isCompleted": false
    }
  ]
}
```

---

## 🛣️ Roadmap

### ✅ Completed
- Core task management
- JWT authentication
- Calendar and scheduling
- Task notes with 1-hour edit window
- Analytics dashboard

### 🔄 In Progress
- Frontend integration

### 📋 Planned
- Task history and audit trail
- Google Calendar OAuth integration
- Two-factor authentication
- File attachments
- Real-time notifications (SignalR)
- Rate limiting
- API versioning

---

## 🤝 Contributing

While this is primarily a learning project, suggestions and feedback are welcome!

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

**Leotero Quirequire**
- LinkedIn: [linkedin.com/in/leotero-quirequire-32ab66156](https://www.linkedin.com/in/leotero-quirequire-32ab66156/)
- GitHub: [@leoleikhiel](https://github.com/leoleikhiel)
- Portfolio: [Coming Soon]

**Background:** PHP Developer transitioning to C#/.NET Backend Engineering

---

## 🙏 Acknowledgments

Built to demonstrate:
- Clean architecture and SOLID principles
- Service layer pattern implementation
- RESTful API best practices
- Entity Framework Core proficiency
- JWT authentication and authorization
- Database design and optimization
- Production-ready code quality

Special focus on understanding **why** architectural decisions are made, not just **how** to implement them.

---

## 📖 Resources

**Official Documentation:**
- [ASP.NET Core Web API](https://docs.microsoft.com/aspnet/core/web-api)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [JWT Authentication in ASP.NET Core](https://docs.microsoft.com/aspnet/core/security/authentication)

**Learning Resources:**
- Clean Architecture by Robert C. Martin
- Microsoft Learn - ASP.NET Core Path
- Entity Framework Core Documentation

---

## 🔗 Related Repositories

- **Frontend UI:** [task-management-ui](https://github.com/leoleikhiel/task-management-ui)

---

**Built with ❤️ using ASP.NET Core 8.0**

*A professional-grade task management API demonstrating enterprise architecture, security best practices, and production-ready development.*

**Version:** 2.0.0  
**Last Updated:** December 2025  
**Status:** Backend Complete - Ready for Integration