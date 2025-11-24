# 📋 Task Management API

A professional-grade RESTful API built with ASP.NET Core 8.0, featuring JWT authentication, service layer architecture, and advanced task management capabilities.

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![Status](https://img.shields.io/badge/status-active%20development-brightgreen)]()

---

## 🚀 Features

### ✅ Implemented Features
- **JWT Authentication** - Secure token-based user authentication
- **User Management** - Registration, login with BCrypt password hashing
- **Task CRUD Operations** - Create, read, update, delete tasks
- **Advanced Filtering** - Search by title, description, status, priority, category
- **User-specific Data** - Complete data isolation per user
- **Analytics Dashboard** - Task statistics and completion insights
- **Service Layer Architecture** - Clean separation of concerns
- **RESTful API Design** - Standard HTTP methods and status codes

### 🔄 In Development
- **Task Notes** - Add detailed notes to tasks with timestamps
- **Calendar Integration** - Schedule and view tasks by date
- **Task History & Audit Trail** - Complete change tracking
- **Google Calendar Sync** - OAuth 2.0 integration and two-way sync
- **AI Category Suggestions** - Smart task categorization
- **Two-Factor Authentication** - Enhanced security with OTP

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

### Current Tables
- **Users** - User accounts with authentication
- **Tasks** - Task items with categories and priorities
- **TaskNotes** - Notes attached to tasks (in development)

### Relationships
- User → Tasks (One-to-Many)
- Task → Notes (One-to-Many)

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
- `GET /api/tasks/search` - Search and filter tasks
- `GET /api/tasks/analytics` - Get task statistics

### Task Notes (Coming Soon)
- `GET /api/tasks/{taskId}/notes` - Get all notes for task
- `POST /api/tasks/{taskId}/notes` - Add note to task
- `PUT /api/tasks/{taskId}/notes/{noteId}` - Update note
- `DELETE /api/tasks/{taskId}/notes/{noteId}` - Delete note

---

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server 2019+ (or LocalDB)
- Visual Studio 2022 or VS Code
- Postman for API testing

### Installation Steps

1. Clone the repository
2. Update connection string in `appsettings.json`
3. Run migrations: `dotnet ef database update`
4. Start the application: `dotnet run`
5. Access Swagger UI at `https://localhost:7001/swagger`

### Configuration
Update `appsettings.json` with your database connection and JWT secret key.

---

## 🔐 Security Features

- **Password Hashing** - BCrypt with automatic salt generation
- **JWT Tokens** - 2-hour expiration, HS256 signing algorithm
- **Data Isolation** - Users can only access their own data
- **Authorization** - All task endpoints require authentication
- **Input Validation** - Data annotations and model validation

---

## 📈 Performance Optimizations

- **Database Indexing** - Optimized queries on UserId, Status, DueDate
- **LINQ Efficiency** - Server-side filtering and sorting
- **Lazy Loading** - Related data loaded only when needed
- **Connection Pooling** - Efficient database connection management

---

## 🛣️ Development Roadmap

### Phase 1: Foundation ✅ (Completed)
- Project setup and configuration
- Database design and migrations
- CRUD operations
- Service layer architecture
- JWT authentication system

### Phase 2: Enhanced Features 🔄 (In Progress)
- Task notes and comments
- Calendar view and scheduling
- Progress tracking
- Task analytics dashboard

### Phase 3: Integrations (Weeks 2-3)
- Google Calendar OAuth 2.0
- Two-way calendar synchronization
- Email notifications
- Webhook support

### Phase 4: Intelligence & Security (Weeks 3-4)
- AI-powered category suggestions
- Task priority recommendations
- Two-factor authentication
- API rate limiting

### Phase 5: Production (Week 4+)
- Azure cloud deployment
- CI/CD pipeline setup
- Application monitoring
- Performance tuning
- API documentation site

---

## 📚 Data Structures & Algorithms

### Concepts Applied
- **Lists & LINQ** - Task filtering and sorting (O(n log n))
- **Dictionaries** - Fast lookups for analytics (O(1))
- **Database Indexing** - B-tree structures for query optimization (O(log n))
- **One-to-Many Relationships** - Efficient foreign key design
- **Query Optimization** - Server-side filtering vs in-memory operations

---

## 🧪 Testing

### Manual Testing
- Postman collection for all endpoints
- Swagger UI for interactive testing
- Test user credentials for quick validation

### Test Scenarios
- User registration and login flow
- Task CRUD operations with authentication
- Search and filter with multiple criteria
- Analytics accuracy verification
- Authorization and data isolation

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

This is a learning project. Feedback and suggestions are welcome!

---

## 📄 License

MIT License - Free to use for learning purposes

---

## 👨‍💻 Author

**Leotero Quirequire**
- LinkedIn: [\[linkedin.com/in/yourprofile\]](https://www.linkedin.com/in/leotero-quirequire-32ab66156/)
- GitHub: [\[github.com/yourusername\]](https://github.com/leoleikhiel)

---

**Built with ❤️ using ASP.NET Core 8.0**

*A professional task management system demonstrating clean architecture, security best practices, and modern API design patterns.*