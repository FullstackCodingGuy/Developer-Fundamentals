# 📌 Expense Manager API

## 🚀 Overview
The **Expense Manager API** is a .NET 9-based application designed to track expenses efficiently. It follows **Clean Architecture** and adheres to **SOLID principles** for maintainability and scalability.

---
## 📌 Features
✅ **CRUD Operations** (Add, Update, Retrieve, Delete Expenses)  
✅ **Expense Categories**  
✅ **SQLite Persistence**  
✅ **Resilient API with Rate Limiting**  
✅ **CORS Policy for Cross-Origin Requests**  
✅ **Logging with Serilog**  
✅ **Performance Optimizations**  
✅ **Centralized Logging with SEQ (Optional)**  

---
## 🛠️ Tech Stack
- **.NET 9** (Minimal API + Controllers)
- **SQLite** (Persistent Storage)
- **Entity Framework Core** (Database ORM)
- **Serilog** (Logging)
- **Rate Limiting** (Resiliency)

---
## 📌 Setup Instructions
### 1️⃣ Clone Repository
```sh
git clone <repository-url>
cd ExpenseManager
```

### 2️⃣ Install Dependencies
```sh
dotnet restore
```

### 3️⃣ Apply Migrations & Initialize Database
```sh
dotnet ef migrations add InitialCreate

dotnet ef database update
```

### 4️⃣ Run the API
```sh
dotnet run
```
The API will be available at **`http://localhost:5000`**

---

### Run the API using Docker
```
make dockerrun

or 

docker run -d -p 8080:8080 --name expense-api-container expense-api

The API will now be accessible at: http://localhost:8080

```

---
## 📌 SQLite Persistence
The API uses **SQLite** for data persistence. The database file (`expense.db`) is automatically created upon running the migrations.

If needed, delete `expense.db` and reapply migrations:
```sh
rm expense.db

dotnet ef database update
```

---
## 📌 Serilog Setup for Logging
Serilog is configured to log messages to **console, files, and SEQ**.
### 1️⃣ Install Serilog Packages
```sh
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Seq
```

### 2️⃣ Configure Logging in `Program.cs`
```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/api-log.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341")
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog();
```

### 3️⃣ Run SEQ for Centralized Logging
If you want to enable **centralized logging**, start SEQ using Docker:
```sh
docker run --name seq -d -e ACCEPT_EULA=Y -p 5341:80 datalust/seq
```
Then, open **http://localhost:5341** to view logs.

---
## 📌 Implemented API Endpoints
| Method | Endpoint           | Description       |
|--------|-------------------|-------------------|
| GET    | `/api/expenses`   | Retrieve expenses |
| POST   | `/api/expenses`   | Add a new expense |
| PUT    | `/api/expenses/{id}` | Update an expense |
| DELETE | `/api/expenses/{id}` | Delete an expense |

---
## 📌 API Resilience - Rate Limiting
To prevent **API abuse**, the following **rate limits** are applied:
- **Max 100 requests per 10 minutes per IP**

Implemented in `Program.cs`:
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", policy => policy
        .PermitLimit(100)
        .Window(TimeSpan.FromMinutes(10)));
});
```

---
## 📌 CORS Policy
To allow **cross-origin requests**, CORS is enabled:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});
```

Activate in the middleware:
```csharp
app.UseCors("AllowAll");
```

---
## 📌 Performance Optimizations
### 🔹 Response Compression
Enable **Gzip/Brotli compression** for faster API responses:
```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});
app.UseResponseCompression();
```

### 🔹 Database Connection Pooling
Optimize database access using **connection pooling**:
```csharp
builder.Services.AddDbContextPool<ExpenseDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
```

---
## 📌 Best Practices Implemented
✅ **SOLID Principles** - Clean Architecture & Decoupled Design  
✅ **Dependency Injection** - Proper Service Layer Usage  
✅ **Rate Limiting** - API Protection  
✅ **Structured Logging** - Serilog + SEQ for Monitoring  
✅ **CORS Policy** - Secure Cross-Origin Access  
✅ **Performance Tweaks** - Response Compression, DB Connection Pooling  

------

### Implementing design patterns in this solution
<details>
<summary>read</summary>



For your **Expense Manager API**, here are some **design patterns** that can be implemented to improve **maintainability, scalability, and testability** while adhering to **SOLID principles**:

---

### 🔹 **1️⃣ Repository Pattern** (Already Implemented)
- **Use Case:** Abstracts database operations, making the API **decoupled** from the persistence logic.
- **Implementation:** The `IExpenseRepository` interface ensures **loose coupling**, and concrete implementations interact with **SQLite via EF Core**.

```csharp
public interface IExpenseRepository
{
    Task<IEnumerable<Expense>> GetAllAsync();
    Task<Expense> GetByIdAsync(int id);
    Task AddAsync(Expense expense);
    Task UpdateAsync(Expense expense);
    Task DeleteAsync(int id);
}
```

✅ **Benefits:**  
✔ Encapsulates database logic  
✔ Makes the app easier to switch databases (e.g., SQL Server, PostgreSQL)  

---

### 🔹 **2️⃣ Unit of Work Pattern** (For Transactional Consistency)
- **Use Case:** When multiple database operations need to be committed together **(e.g., adding an expense + logging the action)**.
- **Implementation:** Wrap repository operations in a **single transaction**.

```csharp
public interface IUnitOfWork
{
    IExpenseRepository Expenses { get; }
    Task<int> SaveChangesAsync();
}
```
✅ **Benefits:**  
✔ Ensures consistency across multiple database operations  
✔ Reduces unnecessary database calls  

---

### 🔹 **3️⃣ Factory Pattern** (For Expense Object Creation)
- **Use Case:** If there are **different types of expenses** (e.g., **Personal, Business, Investment**) and creation logic varies.
- **Implementation:** Use a **factory** to instantiate different expense objects.

```csharp
public static class ExpenseFactory
{
    public static Expense CreateExpense(string type, string title, decimal amount)
    {
        return type switch
        {
            "Personal" => new PersonalExpense(title, amount),
            "Business" => new BusinessExpense(title, amount),
            _ => throw new ArgumentException("Invalid Expense Type")
        };
    }
}
```

✅ **Benefits:**  
✔ Encapsulates object creation logic  
✔ Easier to introduce new expense types  

---

### 🔹 **4️⃣ Strategy Pattern** (For Expense Categorization Logic)
- **Use Case:** If **expense categorization rules** change frequently.
- **Implementation:** Define **multiple categorization strategies** dynamically.

```csharp
public interface IExpenseCategorizationStrategy
{
    string Categorize(Expense expense);
}

public class AmountBasedCategorization : IExpenseCategorizationStrategy
{
    public string Categorize(Expense expense)
    {
        return expense.Amount > 1000 ? "High" : "Low";
    }
}
```

✅ **Benefits:**  
✔ Makes the categorization logic **flexible & interchangeable**  
✔ Avoids **if-else** clutter in the business logic  

---

### 🔹 **5️⃣ CQRS (Command Query Responsibility Segregation)**
- **Use Case:** If the application needs to scale by **separating read and write operations** (e.g., expensive analytics queries).
- **Implementation:** Define **separate** commands (writes) and queries (reads).

```csharp
public record AddExpenseCommand(string Title, decimal Amount);
public record GetExpenseQuery(int Id);
```

✅ **Benefits:**  
✔ Improves **scalability** when using **read replicas**  
✔ Optimizes performance for complex queries  

---

### 🔹 **6️⃣ Decorator Pattern** (For Adding Extra Features Dynamically)
- **Use Case:** If you need to **add logging, caching, or validation** without modifying the core repository logic.
- **Implementation:** Decorate `IExpenseRepository` with **logging functionality**.

```csharp
public class ExpenseRepositoryLoggingDecorator : IExpenseRepository
{
    private readonly IExpenseRepository _inner;
    private readonly ILogger<ExpenseRepositoryLoggingDecorator> _logger;

    public ExpenseRepositoryLoggingDecorator(IExpenseRepository inner, ILogger<ExpenseRepositoryLoggingDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<IEnumerable<Expense>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all expenses.");
        return await _inner.GetAllAsync();
    }
}
```

✅ **Benefits:**  
✔ Adds **cross-cutting concerns** (e.g., logging) without modifying existing code  
✔ Follows **Open/Closed Principle (OCP)**  

---

### 🔹 **7️⃣ Chain of Responsibility Pattern** (For Expense Validation)
- **Use Case:** If multiple **validation steps** need to be executed **sequentially**.
- **Implementation:** Define **linked handlers** for different validation steps.

```csharp
public abstract class ExpenseValidationHandler
{
    protected ExpenseValidationHandler? Next;

    public void SetNext(ExpenseValidationHandler next) => Next = next;

    public abstract void Handle(Expense expense);
}

public class AmountValidationHandler : ExpenseValidationHandler
{
    public override void Handle(Expense expense)
    {
        if (expense.Amount <= 0)
            throw new Exception("Amount must be greater than zero.");

        Next?.Handle(expense);
    }
}
```

✅ **Benefits:**  
✔ Makes the validation **modular** and **extensible**  
✔ Avoids a **massive if-else block**  

---

### 🔹 **8️⃣ Observer Pattern** (For Notifications)
- **Use Case:** If **other services need to react** when an expense is added (e.g., **send email notification**).
- **Implementation:** Use an **event-driven approach**.

```csharp
public class ExpenseNotifier
{
    private readonly List<IObserver> _observers = new();

    public void Subscribe(IObserver observer) => _observers.Add(observer);
    public void Notify(Expense expense) => _observers.ForEach(o => o.Update(expense));
}
```

✅ **Benefits:**  
✔ Allows **event-driven behavior** (e.g., notifying services)  
✔ Enhances **scalability** without modifying the core logic  

---

### 🎯 **Final Thoughts**
| **Pattern** | **Use Case** | **Benefit** |
|------------|-------------|------------|
| **Repository** | Encapsulate database operations | Decouples persistence from business logic |
| **Unit of Work** | Handle transactions | Ensures atomicity & consistency |
| **Factory** | Create different types of expenses | Centralizes object creation logic |
| **Strategy** | Dynamic expense categorization | Avoids complex if-else logic |
| **CQRS** | Separate read & write operations | Enhances scalability |
| **Decorator** | Add logging, caching | Extends behavior without modifying core logic |
| **Chain of Responsibility** | Expense validation | Modular validation steps |
| **Observer** | Notify other services | Enables event-driven architecture |

---


</details>

### Applying Decorator Pattern

<details>
<summary>read</summary>


The **Decorator Pattern** is beneficial in the **Expense Manager API** when you need to **extend the behavior of existing services without modifying them directly**. In this use case, it can help with:  

### 🔹 **Where Can We Use the Decorator Pattern?**
1. **Logging Decorator** – Log every expense-related operation.  
2. **Caching Decorator** – Cache frequent read operations (e.g., fetching expenses).  
3. **Validation Decorator** – Add validation rules dynamically.  
4. **Security/Authorization Decorator** – Check user roles before executing a request.  
5. **Transaction Decorator** – Ensure database consistency.  

---

## **1️⃣ Logging Decorator** (Example)  
Instead of adding logging directly into `ExpenseRepository`, we **wrap** it in a decorator.  

### ✅ **Implementation**
```csharp
public class ExpenseRepositoryLoggingDecorator : IExpenseRepository
{
    private readonly IExpenseRepository _inner;
    private readonly ILogger<ExpenseRepositoryLoggingDecorator> _logger;

    public ExpenseRepositoryLoggingDecorator(IExpenseRepository inner, ILogger<ExpenseRepositoryLoggingDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    public async Task<IEnumerable<Expense>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all expenses.");
        var result = await _inner.GetAllAsync();
        _logger.LogInformation($"Retrieved {result.Count()} expenses.");
        return result;
    }

    public async Task<Expense> GetByIdAsync(int id)
    {
        _logger.LogInformation($"Fetching expense with ID {id}.");
        return await _inner.GetByIdAsync(id);
    }

    public async Task AddAsync(Expense expense)
    {
        _logger.LogInformation($"Adding expense: {expense.Title}, Amount: {expense.Amount}");
        await _inner.AddAsync(expense);
        _logger.LogInformation("Expense added successfully.");
    }

    public async Task UpdateAsync(Expense expense)
    {
        _logger.LogInformation($"Updating expense ID {expense.Id}.");
        await _inner.UpdateAsync(expense);
        _logger.LogInformation("Expense updated successfully.");
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation($"Deleting expense ID {id}.");
        await _inner.DeleteAsync(id);
        _logger.LogInformation("Expense deleted successfully.");
    }
}
```
### ✅ **Usage**
Register the decorated repository in **DI container** in `Program.cs`:
```csharp
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.Decorate<IExpenseRepository, ExpenseRepositoryLoggingDecorator>();
```

📌 **Benefits:**  
✔ **Non-intrusive logging** (no need to modify `ExpenseRepository`)  
✔ **Extensible** (easily add more behaviors)  
✔ **Follows Open/Closed Principle** (OCP)  

---

## **2️⃣ Caching Decorator**  
Reduces **database queries** by caching expenses.

### ✅ **Implementation**
```csharp
public class ExpenseRepositoryCachingDecorator : IExpenseRepository
{
    private readonly IExpenseRepository _inner;
    private readonly IMemoryCache _cache;

    public ExpenseRepositoryCachingDecorator(IExpenseRepository inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<IEnumerable<Expense>> GetAllAsync()
    {
        return await _cache.GetOrCreateAsync("expenses_cache", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _inner.GetAllAsync();
        });
    }

    public async Task<Expense> GetByIdAsync(int id)
    {
        return await _cache.GetOrCreateAsync($"expense_{id}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _inner.GetByIdAsync(id);
        });
    }

    public async Task AddAsync(Expense expense)
    {
        await _inner.AddAsync(expense);
        _cache.Remove("expenses_cache");
    }

    public async Task UpdateAsync(Expense expense)
    {
        await _inner.UpdateAsync(expense);
        _cache.Remove($"expense_{expense.Id}");
    }

    public async Task DeleteAsync(int id)
    {
        await _inner.DeleteAsync(id);
        _cache.Remove($"expense_{id}");
    }
}
```
### ✅ **Usage**
```csharp
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.Decorate<IExpenseRepository, ExpenseRepositoryCachingDecorator>();
```
📌 **Benefits:**  
✔ Reduces **database load**  
✔ Improves **API performance**  
✔ Follows **OCP** (Open/Closed Principle)  

---

## **3️⃣ Validation Decorator**
Ensures **expenses have valid data** before persisting.

### ✅ **Implementation**
```csharp
public class ExpenseRepositoryValidationDecorator : IExpenseRepository
{
    private readonly IExpenseRepository _inner;

    public ExpenseRepositoryValidationDecorator(IExpenseRepository inner)
    {
        _inner = inner;
    }

    public async Task AddAsync(Expense expense)
    {
        if (string.IsNullOrEmpty(expense.Title))
            throw new ArgumentException("Title is required.");
        if (expense.Amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        await _inner.AddAsync(expense);
    }

    public async Task<IEnumerable<Expense>> GetAllAsync() => await _inner.GetAllAsync();
    public async Task<Expense> GetByIdAsync(int id) => await _inner.GetByIdAsync(id);
    public async Task UpdateAsync(Expense expense) => await _inner.UpdateAsync(expense);
    public async Task DeleteAsync(int id) => await _inner.DeleteAsync(id);
}
```
### ✅ **Usage**
```csharp
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.Decorate<IExpenseRepository, ExpenseRepositoryValidationDecorator>();
```
📌 **Benefits:**  
✔ **Keeps validation separate** from repository logic  
✔ **Prevents invalid data from entering the database**  

---

## **🎯 Why Use Decorator Pattern?**
| **Pattern**   | **Use Case**                     | **Benefit**                         |
|--------------|--------------------------------|----------------------------------|
| **Logging**   | Log repository actions         | Non-intrusive, structured logs  |
| **Caching**   | Reduce DB calls for reads      | Improves performance            |
| **Validation** | Validate expenses before saving | Keeps concerns separate          |

📌 **Best Part?** ✅ You can **stack multiple decorators together!**  
Example:
```csharp
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.Decorate<IExpenseRepository, ExpenseRepositoryValidationDecorator>();
builder.Services.Decorate<IExpenseRepository, ExpenseRepositoryCachingDecorator>();
builder.Services.Decorate<IExpenseRepository, ExpenseRepositoryLoggingDecorator>();
```
✔ **Validation → Caching → Logging** in order 🔄  

---

### **🚀 Summary**
🔹 The **Decorator Pattern** helps in **adding cross-cutting concerns dynamically**.  
🔹 **No need to modify core repository code** – just wrap and extend!  
🔹 Follows **OCP (Open/Closed Principle)** – Code is **open for extension, closed for modification**.  
🔹 Improves **maintainability & testability**.  

Would you like help implementing **unit tests** for these decorators? 🚀

</details>

---

## Implementing Authentication

- https://github.com/FullstackCodingGuy/Developer-Fundamentals/wiki/Authentication#implementing-keycloak-in-a-net-9-api-solution

---
## 📌 License
This project is open-source and available under the **MIT License**.

---
## 🎯 **Final Notes**
This API is production-ready, **resilient**, and **scalable**. Feel free to customize it for your needs!

💬 **Need Help?** Reach out for support! 🚀

