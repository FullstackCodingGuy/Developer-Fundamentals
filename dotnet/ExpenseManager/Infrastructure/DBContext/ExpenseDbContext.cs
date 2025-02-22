using ExpenseManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ExpenseManager.Infrastructure.Persistence
{
    public class ExpenseDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options, IConfiguration configuration) 
        : base(options) 
    {
        _configuration = configuration;
    }

    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlite(connectionString, b => 
                b.MigrationsAssembly("ExpenseManager.Infrastructure")); // Ensure migrations are stored in Infrastructure
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Expense>().ToTable("Expenses");
        modelBuilder.Entity<Category>().ToTable("Categories");
    }
}
}
