Here’s an example of how to structure a .NET Clean Architecture project where the SQLite database file (`sqlite.db`) is stored in the `App_Data` folder of the **API project**, while the `DbContext` and migration files are kept in a separate **Infrastructure** project.

---

### **Folder Structure**
```
- SolutionName
  - Domain
    - Entities
    - ValueObjects
  - Application
    - Interfaces
    - Services
    - UseCases
  - Infrastructure
    - Data
      - ApplicationDbContext.cs
      - Migrations/
  - WebAPI (Main API Project)
    - App_Data
      - sqlite.db
    - Controllers
    - Program.cs
    - appsettings.json
```

---

### **Step-by-Step Implementation**

#### 1. **Create the `App_Data` Folder in the API Project**
- In the **WebAPI** project, create a folder named `App_Data`.
- This folder will hold the `sqlite.db` file.

---

#### 2. **Configure the Connection String in `appsettings.json`**
In the **WebAPI** project, configure the SQLite connection string to point to the `App_Data` folder.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=App_Data/sqlite.db"
  }
}
```

---

#### 3. **Set Up the `DbContext` in the Infrastructure Project**
In the **Infrastructure** project, create the `ApplicationDbContext` class.

```csharp
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define your DbSets here
        public DbSet<YourEntity> YourEntities { get; set; }
    }
}
```

---

#### 4. **Add Migrations in the Infrastructure Project**
Run the following commands to add migrations and specify the output directory for migration files:

```bash
dotnet ef migrations add InitialMigration -o Data/Migrations --project Infrastructure --startup-project WebAPI
```

- `--project Infrastructure`: Specifies the project where the `DbContext` is located.
- `--startup-project WebAPI`: Specifies the project where the application starts (and where the `appsettings.json` file is located).
- `-o Data/Migrations`: Specifies the folder in the **Infrastructure** project where migration files will be stored.

---

#### 5. **Configure the `DbContext` in the API Project**
In the **WebAPI** project, configure the `DbContext` in `Program.cs` to use the connection string.

```csharp
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Apply migrations at runtime (optional)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

app.MapControllers();
app.Run();
```

---

#### 6. **Ensure the `App_Data` Folder is Copied to the Output Directory**
To ensure the `App_Data` folder is included in the build output, update the **WebAPI** project’s `.csproj` file:

```xml
<ItemGroup>
  <Content Include="App_Data\**\*" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

---

#### 7. **Run the Application**
- Run the following commands to apply migrations and create the SQLite database in the `App_Data` folder:
  ```bash
  dotnet ef database update --project Infrastructure --startup-project WebAPI
  ```

- Start the application, and the `sqlite.db` file will be created in the `App_Data` folder of the **WebAPI** project.

---

### **Final Notes**
- The **Infrastructure** project contains the `DbContext` and migration files, keeping database-related concerns separate from the API project.
- The **WebAPI** project holds the `App_Data` folder and connection string, ensuring the database file is stored in a secure and accessible location.
- This setup adheres to Clean Architecture principles by maintaining a clear separation of concerns.
