# 📋 Task Management API

A professional-grade RESTful API built with ASP.NET Core 8.0, featuring JWT authentication, service layer architecture, and advanced task management capabilities including calendar system and note-taking.

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![Status](https://img.shields.io/badge/status-backend%20complete-brightgreen)]()

---

## 🚀 Features

### ✅ Current Features (Fully Implemented)

#### Core Task Management
- **JWT Authentication** - Secure token-based user authentication with BCrypt password hashing
- **User Management** - Registration, login, and role-based access control
- **Task CRUD Operations** - Create, read, update, delete tasks with validation
- **Advanced Filtering** - Search by title, description, status, priority, category
- **User-specific Data** - Complete data isolation per user with security checks
- **Analytics Dashboard** - Task statistics, completion rates, and category breakdowns

#### Task Notes System
- **Note Creation** - Add detailed notes to any task with timestamps
- **Note Management** - Update and delete notes with validation
- **1-Hour Edit Window** - Business rule: notes editable within 1 hour of creation
- **Note History** - View all notes for a task, sorted by newest first
- **Cascade Delete** - Notes automatically deleted when parent task is removed

#### Calendar & Scheduling
- **Task Scheduling** - Set ScheduledDate (when to work) and DueDate (deadline)
- **Smart Display Logic** - Tasks appear by ScheduledDate with DueDate fallback
- **Multiple Calendar Views:**
  - Today's tasks (scheduled or due today)
  - This week's tasks (Monday-Sunday)
  - Overdue tasks (sorted by urgency)
  - Calendar grouped by date (with 90-day range limit)
  - Month-based calendar helper
- **Completion Tracking** - Hybrid auto/manual CompletedAt timestamp
- **Overdue Detection** - Automatic calculation with computed property

### Architecture & Code Quality
- **Service Layer Pattern** - Clean separation of concerns
- **Dependency Injection** - Interface-based design throughout
- **DTO Pattern** - Request/response data transfer objects
- **Database Indexing** - Optimized queries with B-tree indexes
- **Input Validation** - Comprehensive validation with helpful error messages
- **RESTful Design** - Standard HTTP methods and status codes
- **Multi-User Support** - Complete data isolation and security

---

## 🏗️ Architecture

### Design Patterns
- **Service Layer Pattern** - Business logic separated from controllers
- **Repository Pattern** - Data access through Entity Framework Core
- **Dependency Injection** - Interface-based design for loose coupling
- **DTO Pattern** - Request/response data transfer objects

### Project Structure
```
TaskManagementAPI/
├── Controllers/        # HTTP endpoints
├── Services/          # Business logic
├── Models/            # Database entities
├── DTOs/              # Data transfer objects
├── Data/              # DbContext and migrations
└── Program.cs         # Configuration and DI
```

---

## 🛠️ Tech Stack

- **Framework:** ASP.NET Core 8.0
- **Database:** SQL Server with Entity Framework Core
- **Authentication:** JWT Bearer Tokens
- **Password Security:** BCrypt.Net
- **API Documentation:** Swagger/OpenAPI

---

## 📊 Database Schema

### Current Schema

```sql
-- Users Table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    UserRole INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    INDEX IX_Users_Email (Email)
);

-- Categories Table
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    UserId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

-- Tasks Table
CREATE TABLE Tasks (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(1000),
    IsCompleted BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    DueDate DATETIME2,
    ScheduledDate DATETIME2,
    CompletedAt DATETIME2,
    UserId INT NOT NULL,
    CategoryId INT,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    INDEX IX_Tasks_UserId (UserId),
    INDEX IX_Tasks_DueDate (DueDate)
);

-- TaskNotes Table
CREATE TABLE TaskNotes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Content NVARCHAR(2000) NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    TaskId INT NOT NULL,
    FOREIGN KEY (TaskId) REFERENCES Tasks(Id) ON DELETE CASCADE,
    INDEX IX_TaskNotes_TaskId_CreatedAt (TaskId, CreatedAt)
);
```

### Key Design Decisions

**Indexes:**
- `DueDate` - Optimizes overdue queries from O(n) to O(log n)
- `(TaskId, CreatedAt)` - Composite index for note retrieval and sorting
- `Email` - Unique constraint with index for fast authentication lookups

**Relationships:**
- User → Tasks (One-to-Many with CASCADE delete)
- User → Categories (One-to-Many with CASCADE delete)
- Task → Notes (One-to-Many with CASCADE delete)
- Category → Tasks (One-to-Many, SET NULL on delete)

**Computed Properties (Not Stored):**
- `IsOverdue` - Calculated: `!IsCompleted && DueDate < Today`
- Saves space and ensures always accurate

---

## 🔌 API Endpoints

### Authentication
- `POST /api/auth/register` - Create new user account
- `POST /api/auth/login` - Login and receive JWT token

### Tasks
- `GET /api/tasks` - Get all user tasks
- `GET /api/tasks/{id}` - Get specific task
- `POST /api/tasks` - Create new task
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task
- `GET /api/tasks/search?title={query}` - Search tasks by title
- `GET /api/tasks/filter?isCompleted={bool}&categoryId={id}` - Filter tasks
- `GET /api/tasks/statistics` - Get task analytics
- `PUT /api/tasks/complete-all` - Mark all tasks as complete

### Calendar & Scheduling
- `GET /api/tasks/today` - Tasks scheduled or due today
- `GET /api/tasks/week` - Tasks for current week (Mon-Sun)
- `GET /api/tasks/overdue` - Overdue tasks sorted by urgency
- `GET /api/tasks/calendar?startDate={date}&endDate={date}` - Calendar view grouped by date (max 90 days)
- `GET /api/tasks/calendar/month?month={int}&year={int}` - Month-based calendar helper

### Task Notes
- `GET /api/tasks/{taskId}/notes` - Get all notes for task
- `GET /api/tasks/{taskId}/notes/{noteId}` - Get specific note
- `POST /api/tasks/{taskId}/notes` - Add note to task
- `PUT /api/tasks/{taskId}/notes/{noteId}` - Update note (1-hour window)
- `DELETE /api/tasks/{taskId}/notes/{noteId}` - Delete note

### Categories
- `GET /api/categories` - Get all user categories
- `POST /api/categories` - Create new category
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category

**Total Endpoints:** 24 REST endpoints

---

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server 2019+ (or LocalDB)
- Visual Studio 2022 or VS Code
- Postman for API testing

### Installation Steps

1. **Clone the repository**
```bash
git clone https://github.com/leoleikhiel/task-management-api.git
cd task-management-api
```

2. **Update connection string in `appsettings.json`**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TaskManagementDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "your-secret-key-at-least-32-characters-long",
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
Navigate to: `https://localhost:7001/swagger`

### ⚠️ Configuration
- Update `appsettings.json` with your database connection string
- Generate a secure JWT secret key (minimum 32 characters)
- Never commit real secrets to version control!

---

## 🔐 Security Features

- **Password Hashing** - BCrypt with automatic salt generation
- **JWT Tokens** - 2-hour expiration, HS256 signing algorithm
- **Data Isolation** - Users can only access their own data
- **Authorization** - All endpoints require authentication
- **Input Validation** - Comprehensive validation with helpful error messages
- **1-Hour Edit Window** - Time-based business rule for note editing

---

## 📈 Performance Considerations

### Database Indexing Strategy
```csharp
// Primary indexes for common queries
IX_Users_Email           // O(1) authentication lookups
IX_Tasks_UserId          // O(log n) user task filtering
IX_Tasks_DueDate         // O(log n) overdue/calendar queries
IX_TaskNotes_TaskId_CreatedAt  // Composite: O(log n) + sorted retrieval
```

### Query Optimization Examples

**Overdue Tasks Query:**
```
Without index: O(n) - Full table scan
With DueDate index: O(log n + k) - B-tree lookup + k results
For 10,000 tasks, 50 overdue: 10,000 ops → 50 ops (200x faster!)
```

**Calendar Grouping:**
```
Algorithm: Fetch filtered tasks + GroupBy in memory
Time: O(m) where m = tasks in date range
Space: O(m) for dictionary
Optimal: Can't do better than O(m) - must examine each task
```

### Time Complexity Summary
- **Get tasks:** O(log n) with index lookup
- **Create task:** O(1) insertion
- **Update task:** O(1) with primary key
- **Delete task:** O(1) with cascade
- **Search tasks:** O(n) linear scan
- **Calendar grouping:** O(m) where m = filtered tasks
- **Week calculation:** O(m) filtered tasks

---

## 📚 Data Structures & Algorithms Used

### Data Structures
- **Lists** - Task collections, O(n) operations with LINQ
- **Dictionaries/HashMaps** - GroupBy creates hash tables for O(1) lookups
- **B-Trees** - Database indexes for O(log n) searches
- **Navigation Properties** - EF Core relationship management

### Algorithms Implemented
- **Filtering** - WHERE clauses with index optimization
- **Sorting** - OrderBy operations (O(n log n) QuickSort)
- **Grouping** - GroupBy implementation using hash tables (O(n))
- **Date Arithmetic** - Week calculations, range validation
- **Search** - Linear scan for text search (O(n))

### Complexity Analysis

**Service Method Complexities:**
```
GetAllTasksAsync:          O(n) where n = user's tasks
GetTaskByIdAsync:          O(1) with primary key index
CreateTaskAsync:           O(1) insertion
UpdateTaskAsync:           O(1) with primary key
DeleteTaskAsync:           O(1) with primary key
GetTasksForTodayAsync:     O(m) where m = today's tasks
GetTasksForWeekAsync:      O(m) where m = week's tasks
GetOverdueTasksAsync:      O(k log k) where k = overdue (sorted)
GetTasksGroupedByDateAsync: O(n + g log g) ≈ O(n)
                           n = filtered tasks, g = distinct dates
```

**All algorithms are optimal** - Cannot do better than O(n) when processing n items!

---

## 🧪 Testing

### Test Data Seeder
Comprehensive database seeder with realistic scenarios:
- 2 users (John Doe - Regular, Jane Smith - Admin)
- 8 categories across both users
- 22 tasks covering all scenarios:
  - Today's tasks (2)
  - This week (5)
  - This month (3)
  - Next month (2)
  - Overdue (3 tasks: 10, 5, 3 days ago)
  - Completed (4)
  - Cross-user data (3 for Jane)
- Multiple notes per task with realistic content

**Seeder Features:**
- Auto-runs in development environment
- Clears existing data safely (respects foreign keys)
- BCrypt password hashing
- Realistic dates relative to current day
- Smart note content generation

**Test Credentials:**
```
User 1 (Regular):
Email: john.doe@example.com
Password: Password123!

User 2 (Admin):
Email: jane.smith@example.com  
Password: Password456!
```

### Manual Testing Checklist
- ✅ Authentication flow (register, login)
- ✅ Task CRUD operations
- ✅ Search and filtering
- ✅ Today/week/overdue endpoints
- ✅ Calendar grouping
- ✅ Note creation and editing
- ✅ 1-hour edit window validation
- ✅ User data isolation
- ✅ Date range validation (90-day limit)
- ✅ Error handling and validation

### API Testing Tools
- Swagger UI: `https://localhost:7001/swagger`
- Postman collection available
- All endpoints include authentication

---

## 🛣️ Roadmap

### Phase 1: Foundation ✅ (Completed)
- [x] Project setup and configuration
- [x] Database design with EF Core
- [x] CRUD operations for tasks
- [x] Service layer architecture
- [x] JWT authentication system
- [x] User registration and login
- [x] Advanced search and filtering
- [x] Analytics dashboard

### Phase 2: Enhanced Features ✅ (Completed)
- [x] Task notes with 1-hour edit window
- [x] Calendar tagging and scheduling
- [x] Multiple calendar views (today/week/overdue)
- [x] Smart display date logic
- [x] Date range validation
- [x] Completion tracking with hybrid CompletedAt
- [x] Comprehensive test data seeder

### Phase 3: Frontend Integration 🔄 (Next)
- [ ] React/Vue.js UI
- [ ] Calendar component
- [ ] Task management interface
- [ ] Authentication flow
- [ ] Responsive design

### Phase 4: Advanced Features (Future)
- [ ] Task history and audit trail
- [ ] Google Calendar OAuth integration
- [ ] Two-way calendar sync
- [ ] AI-powered category suggestions
- [ ] Two-factor authentication
- [ ] File attachments

### Phase 5: Production Deployment (Future)
- [ ] Azure App Service deployment
- [ ] CI/CD pipeline with GitHub Actions
- [ ] Application monitoring
- [ ] Performance optimization
- [ ] API rate limiting
- [ ] Comprehensive documentation site

---

## 🚀 Deployment

### Azure App Service (Planned)
- SQL Server database
- App Service for API hosting
- Application Insights for monitoring
- Automated CI/CD with GitHub Actions

---

## 📄 API Documentation

Full API documentation available via Swagger UI when running the application.

Interactive documentation: `https://localhost:7001/swagger`

---

## 🤝 Contributing

This is a learning project demonstrating professional API development practices. Feedback and suggestions are welcome!

---

## 📄 License

MIT License - Free to use for learning purposes

---

## 👨‍💻 Author

**Leotero Quirequire**
- LinkedIn: [linkedin.com/in/leotero-quirequire-32ab66156](https://www.linkedin.com/in/leotero-quirequire-32ab66156/)
- GitHub: [github.com/leoleikhiel](https://github.com/leoleikhiel)

**Developer Journey:** PHP Developer → C#/.NET Backend Engineer

**Learning Focus:** 
- Enterprise architecture patterns
- Service layer design
- Data structures & algorithms
- RESTful API best practices
- Clean code principles

---

## 🙏 Acknowledgments

Built as a learning project demonstrating:
- ASP.NET Core Web API development
- Entity Framework Core with Code-First approach
- JWT authentication and authorization
- Service layer architecture patterns
- RESTful design principles
- Data structures and algorithm optimization
- Real-world edge case handling

Special focus on understanding **why** behind architectural decisions, not just **how** to implement features.

---

## 📖 Additional Resources

**Microsoft Documentation:**
- [ASP.NET Core Web API](https://docs.microsoft.com/aspnet/core/web-api)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [JWT Authentication](https://docs.microsoft.com/aspnet/core/security/authentication)

**Architecture Patterns:**
- Clean Architecture by Robert C. Martin
- Domain-Driven Design principles
- SOLID design patterns

---

**Built with ❤️ using ASP.NET Core 8.0**

*A professional task management system demonstrating clean architecture, security best practices, modern API design patterns, and production-ready code quality.*

**Last Updated:** November 2024  
**Status:** Backend Complete - Ready for Frontend Integration  
**Version:** 2.0.0 (Task Notes + Calendar Features)