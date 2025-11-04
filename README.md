# 🏢 EmployeeSystem Web API — EF Core + SQL Server

A clean and modular **ASP.NET Core Web API** for managing employees and departments.  
The project follows **SOLID principles**, uses **Entity Framework Core + SQL Server**, and exposes RESTful endpoints with Swagger UI.

---

## ✨ Features

### ✅ Employees
- Create / Read / Update / Delete
- Filter:
  - Active employees
  - Inactive employees
  - By department
  - By position
  - By minimum years of service
- Patch support (`UpdatePartial`)
- Auto-calculate `YearsOfService`
- Deactivate employee endpoint

### ✅ Departments
- Full CRUD
- Prevent delete if employees exist
- Get employees in specific department

---

## 🧠 Architecture & Design

| Concept | Applied |
|--------|--------|
Clean Architecture | ✅ Controllers → Services → EF Core |
SOLID Principles | ✅ SRP, DIP, DI |
Dependency Injection | ✅ Services registered in Program.cs |
DTOs | ✅ For partial update (PATCH) |
EF Core | ✅ Code-First, Migrations, Async ops |
Circular JSON Fix | ✅ `JsonIgnore` on navigation properties |

---
## 📂 Project Structure

```console
EmployeeSystem/
│
├── Controllers/
│   ├── EmployeesController.cs
│   └── DepartmentsController.cs
│
├── Data/
│   ├── AppDbContext.cs
│   └── AppDbContextSeed.cs
│
├── Dtos/
│   └── UpdateEmployeeDto.cs
│
├── Models/
│   ├── EmployeeModel.cs
│   └── DepartmentModel.cs
│
├── Services/
│   ├── Interfaces/
│   │   ├── IEmployeeService.cs
│   │   └── IDepartmentService.cs
│   │
│   ├── EmployeeService.cs
│   └── DepartmentService.cs
│
├── Migrations/
│
├── Program.cs
└── appsettings.json


```

## 📑 API Endpoints

### 👥 Employees Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
GET | `/api/employees` | Get all employees  
GET | `/api/employees/active` | Get active employees  
GET | `/api/employees/inactive` | Get inactive employees  
GET | `/api/employees/{id}` | Get employee by ID  
POST | `/api/employees` | Create a new employee  
PUT | `/api/employees/{id}` | Update employee (full update)  
PATCH | `/api/employees/{id}` | Partially update employee  
DELETE | `/api/employees/{id}` | Delete an employee  
POST | `/api/employees/{id}/deactivate?endDate=YYYY-MM-DD` | Deactivate employee and set EndOfServiceDate  
GET | `/api/employees/by-department?departmentId={id}` | List employees in specific department  
GET | `/api/employees/by-position?position=Manager` | Filter employees by position  
GET | `/api/employees/min-years?minYears=3` | Employees with minimum years of service  


---

### 🏢 Department Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
GET | `/api/departments` | Get all departments  
GET | `/api/departments/{id}` | Get department by ID  
POST | `/api/departments` | Create a new department  
PUT | `/api/departments/{id}` | Update department  
DELETE | `/api/departments/{id}` | Delete department (blocked if employees exist)  
GET | `/api/departments/{id}/employees` | Get employees in a department  

