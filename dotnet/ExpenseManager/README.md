# Expense Manager API

## 📌 Project Overview
The **Expense Manager API** is a backend service built with **.NET 9** that allows users to track their expenses efficiently. The project follows **Clean Architecture** and **SOLID principles**, ensuring a well-structured and maintainable codebase.

---

## 🚀 Features

### **Expense Management**
- ✅ Add a new expense with **title** (mandatory), **description** (optional), **date** (optional), **amount** (mandatory), and **category**.
- ✅ Update an existing expense.
- ✅ Retrieve all expenses or filter them by date/category.
- ✅ Delete an expense.

### **Database & Persistence**
- ✅ Uses **SQLite** as the database.
- ✅ Implements **Entity Framework Core** for data management.
- ✅ Supports **Migrations** to keep the database schema up to date.

### **API Design**
- ✅ Built using **Minimal API** and **Controllers**.
- ✅ Well-structured **DTOs (Data Transfer Objects)** to manage API requests/responses.
- ✅ Uses **Dependency Injection** for better maintainability.

### **Resilience & Error Handling**
- ✅ Implements **global exception handling** for better debugging.
- ✅ Uses **try-catch blocks** and **logging** for tracking errors.

---

## 📂 Project Structure
```
ExpenseManager/
│── ExpenseManager.API/          # API Project (Entry Point)
│── ExpenseManager.Application/  # Business Logic Layer
│── ExpenseManager.Domain/       # Entities & Core Logic
│── ExpenseManager.Infrastructure/ # Database & Persistence Layer
│── README.md                    # Project Documentation
```

---

## 🔧 Setup & Installation

### **1️⃣ Prerequisites**
- Install **.NET 9 SDK**
- Install **SQLite** (if not already installed)

### **2️⃣ Clone the Repository**
```sh
git clone https://github.com/your-repo/ExpenseManager.git
cd ExpenseManager
```

### **3️⃣ Install Dependencies**
Navigate to the API project and restore dependencies:
```sh
dotnet restore
```

### **4️⃣ Apply Database Migrations**
```sh
dotnet ef database update --project ExpenseManager.Infrastructure --startup-project ExpenseManager.API
```

### **5️⃣ Run the Application**
```sh
dotnet run --project ExpenseManager.API
```
The API should now be running on **http://localhost:5000** (or another assigned port).

---

## 📡 API Endpoints

| HTTP Method | Endpoint | Description |
|------------|---------|-------------|
| **POST** | `/api/expenses` | Add a new expense |
| **GET** | `/api/expenses` | Retrieve all expenses |
| **GET** | `/api/expenses/{id}` | Get a single expense by ID |
| **PUT** | `/api/expenses/{id}` | Update an expense |
| **DELETE** | `/api/expenses/{id}` | Delete an expense |

---

## 🛠 Technologies Used
- **.NET 9**
- **Entity Framework Core**
- **SQLite**
- **Minimal API + Controllers**
- **Dependency Injection**
- **Clean Architecture**

---

## 📌 Future Enhancements
- 🔹 **User authentication & authorization** (JWT)
- 🔹 **Expense categories & reports**
- 🔹 **Export expenses to CSV/PDF**
- 🔹 **Cloud database support** (PostgreSQL, SQL Server)

---

## 📄 License
This project is licensed under the MIT License.

---

## 🤝 Contributing
Contributions are welcome! Feel free to open an issue or submit a pull request. 🎯

