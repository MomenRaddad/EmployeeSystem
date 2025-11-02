# 🏢 EmployeeSystem Web API

The **EmployeeSystem** is a robust **ASP.NET Core Web API** designed for managing employee and department data. It follows a clean, service-oriented architecture, using an **In-Memory Store** for data persistence. This project demonstrates strong separation of concerns and integrated business logic.

---

## ✨ Key Features & Business Logic

The system is fully compliant with all specified requirements, offering the following functionality:

* **Complete CRUD:** Full Create, Read, Update, and Delete operations for both **Employee** and **Department** models.
* **Automatic Service Calculation:** The read-only property **`YearsOfService`** is dynamically calculated based on the employee's `DateOfEmployment`.
* **Advanced Employee Querying:** Retrieval of **active/inactive** employees, filtering by **Department ID**, **Position**, and **minimum years of service**.
* **Data Integrity & Validation:**
    * **Required Field Validation** is enforced on key model properties.
    * **Deletion Lock:** Prevents deletion of a department if any employees are linked to it.
* **Employee Lifecycle:** A dedicated endpoint to **deactivate** an employee, which sets `IsActive` to `false` and records the `EndOfServiceDate`.

---

## 🛠️ Prerequisites

* **.NET SDK** (The target framework used for the project).
* **Visual Studio** or **Visual Studio Code**.

---

## ⚙️ Getting Started

1.  **Clone the Repository:**
    ```bash
    git clone [https://github.com/MomenRaddad/EmployeeSystem.git](https://github.com/MomenRaddad/EmployeeSystem.git) 
    cd EmployeeSystem
    ```
2.  **Run the Application:**
    ```bash
    dotnet run
    ```
The API will launch on a local port (e.g., `https://localhost:7166`).

### 🔗 API Documentation & Testing

Once running, the entire API can be viewed and tested interactively using the **Swagger UI**:

* **Swagger URL:** `https://localhost:7166/swagger/index.html` (Note: The port may vary.)

---

## 🗺️ Project Structure

The project uses a clean architecture with clear layer separation:

```markdown
EmployeeSystem/
├── Controllers/         # Handles incoming HTTP requests and calls the relevant services
│   ├── EmployeesController.cs  
│   └── DepartmentsController.cs
|
├── Models/              # Data Entities (EmployeeModel, DepartmentModel)
│   ├── EmployeeModel.cs  
│   └── DepartmentModel.cs
|
├── Services/            # Business Logic Layer (Implements core application logic)
│   ├── Interfaces/      # Service Contracts (IEmployeeService, IDepartmentService)
│   └── (Concrete Services) # Service implementations
|
├── Data/                # Data Access Layer
│   ├── json/            # Folder containing the persistent JSON data files
│   │   ├── employees.json   # Employee data store
│   │   └── departments.json # Department data store
│   └── InMemoryStore.cs # Class responsible for reading/writing data from/to JSON files (Persistent In-Memory Store)
|
├── Program.cs           # Application entry point, service registration (DI), and middleware configuration
└── appsettings.json     # Application configuration settings
