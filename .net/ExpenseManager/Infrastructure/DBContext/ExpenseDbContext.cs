using ExpenseManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManager.Infrastructure.Persistence
{
    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options) { }

        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasColumnType("decimal(18,2)");

            base.OnModelCreating(modelBuilder);
        }
    }
}
