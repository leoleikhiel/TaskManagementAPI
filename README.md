# 📝 Task Management API

A comprehensive RESTful API for managing tasks, categories, and products with advanced filtering, search capabilities, and analytics dashboard.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12-239120?logo=csharp)
![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-CC2927?logo=microsoftsqlserver)
![License](https://img.shields.io/badge/License-MIT-green)

---

## 🚀 Features

### Core Functionality
- ✅ **Complete CRUD Operations** - Tasks, Categories, and Products
- ✅ **Task-Category Relationships** - Organize tasks by category
- ✅ **Due Date Tracking** - Never miss a deadline
- ✅ **Completion Management** - Track task progress

### Advanced Features
- 🔍 **Smart Search** - Find tasks by title
- 🎯 **Advanced Filtering** - Filter by status, category, or both
- ⏰ **Overdue Detection** - Automatically identify overdue tasks
- 📊 **Real-time Analytics** - Completion rates and category statistics
- ⚡ **Bulk Operations** - Complete all tasks at once
- ✅ **Input Validation** - Data integrity enforcement

---

## 🛠️ Tech Stack

| Technology | Purpose |
|------------|---------|
| **ASP.NET Core 8.0** | Web API Framework |
| **Entity Framework Core 8.0** | ORM |
| **SQL Server LocalDB** | Database |
| **C# 12** | Programming Language |
| **Swagger/OpenAPI** | API Documentation |

**Architecture:** RESTful API with Repository Pattern (via DbContext)

---

## 📋 API Endpoints

### 📝 Tasks (11 Endpoints)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/tasks` | Get all tasks |
| `GET` | `/api/tasks/{id}` | Get task by ID |
| `POST` | `/api/tasks` | Create new task |
| `PUT` | `/api/tasks/{id}` | Update task |
| `DELETE` | `/api/tasks/{id}` | Delete task |
| `PUT` | `/api/tasks/complete-all` | Mark all tasks complete |
| `GET` | `/api/tasks/search?title={query}` | Search by title |
| `GET` | `/api/tasks/filter?isCompleted={bool}&categoryId={id}` | Filter tasks |
| `GET` | `/api/tasks/overdue` | Get overdue tasks |
| `GET` | `/api/tasks/statistics` | Get analytics |

### 📁 Categories (6 Endpoints)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/categories` | Get all categories |
| `GET` | `/api/categories/{id}` | Get category by ID |
| `POST` | `/api/categories` | Create category |
| `PUT` | `/api/categories/{id}` | Update category |
| `DELETE` | `/api/categories/{id}` | Delete category |
| `GET` | `/api/categories/{id}/tasks` | Get tasks in category |

### 📦 Products (5 Endpoints)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/products` | Get all products |
| `GET` | `/api/products/{id}` | Get product by ID |
| `POST` | `/api/products` | Create product |
| `PUT` | `/api/products/{id}` | Update product |
| `DELETE` | `/api/products/{id}` | Delete product |

**Total: 22 Endpoints** 🎯

---

## 🔧 Installation & Setup

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Steps

1. **Clone the repository**
```bash
git clone https://github.com/leoleikhiel/TaskManagementAPI.git
cd TaskManagementAPI
```

2. **Configure database connection**
```bash
# Copy template and update connection string
cp appsettings.Template.json appsettings.json
```

Update `appsettings.json` with your SQL Server details.

3. **Run database migrations**
```bash
dotnet ef database update
```

4. **Run the application**
```bash
dotnet run
```

5. **Access the API**
- **Swagger UI:** `https://localhost:7081/swagger`
- **Base URL:** `https://localhost:7081/api`

---

## 📊 Example Usage

### Create a Task
```bash
POST /api/tasks
Content-Type: application/json

{
    "title": "Complete API documentation",
    "description": "Write comprehensive README",
    "categoryId": 1,
    "dueDate": "2025-12-31T23:59:59Z"
}
```

**Response (201 Created):**
```json
{
    "id": 1,
    "title": "Complete API documentation",
    "description": "Write comprehensive README",
    "isCompleted": false,
    "createdAt": "2025-11-22T00:00:00Z",
    "dueDate": "2025-12-31T23:59:59Z",
    "categoryId": 1,
    "category": {
        "id": 1,
        "name": "Work"
    }
}
```

### Get Statistics
```bash
GET /api/tasks/statistics
```

**Response (200 OK):**
```json
{
    "total": 15,
    "completedTasks": 10,
    "incompleteTasks": 5,
    "completionRate": 66.67,
    "byCategory": [
        {
            "category": "Work",
            "count": 8
        },
        {
            "category": "Personal",
            "count": 5
        },
        {
            "category": "Uncategorized",
            "count": 2
        }
    ]
}
```

### Search Tasks
```bash
GET /api/tasks/search?title=documentation
```

### Filter Tasks
```bash
GET /api/tasks/filter?isCompleted=false&categoryId=1
```

---

## 🗄️ Database Schema

### Tasks
| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary Key (auto-increment) |
| Title | nvarchar(100) | Task title (required) |
| Description | nvarchar(500) | Task description |
| IsCompleted | bit | Completion status |
| CreatedAt | datetime2 | Creation timestamp |
| DueDate | datetime2 | Due date (optional) |
| CategoryId | int | Foreign Key to Categories |

### Categories
| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary Key (auto-increment) |
| Name | nvarchar(50) | Category name (required) |

### Products
| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary Key (auto-increment) |
| Name | nvarchar(max) | Product name |
| Price | float | Product price |
| Stock | int | Stock quantity |

---

## ✅ Input Validation

All DTOs include validation rules:

- **Task Title:** Required, 3-100 characters
- **Task Description:** Max 500 characters
- **Category Name:** Required, 2-50 characters
- **Due Date:** Optional, must be valid DateTime

Invalid requests return `400 Bad Request` with detailed error messages.

---

## 🎯 Roadmap

### Coming Soon
- [ ] **JWT Authentication** - Secure user access
- [ ] **User Management** - Registration and login
- [ ] **Role-Based Authorization** - Admin and user roles
- [ ] **Email Notifications** - Due date reminders
- [ ] **File Attachments** - Attach files to tasks
- [ ] **Pagination** - Handle large datasets
- [ ] **React Frontend** - Web interface
- [ ] **Azure Deployment** - Cloud hosting

---

## 👨‍💻 Author

**Leo Leikhiel**

- GitHub: [@leoleikhiel](https://github.com/leoleikhiel)
- Project: [TaskManagementAPI](https://github.com/leoleikhiel/TaskManagementAPI)

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- Built with [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
- Database powered by [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- API documentation via [Swagger/OpenAPI](https://swagger.io/)

---

## 📧 Contact

For questions or feedback, please open an issue on GitHub.

---

<div align="center">

**⭐ Star this repository if you found it helpful!**

Made with ❤️ and C#

</div>