using ExpenseManager.Application.Interfaces;
using ExpenseManager.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ExpenseManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace ExpenseManager.Infrastructure.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ExpenseDbContext _context;

        public ExpenseRepository(ExpenseDbContext context)
        {
            _context = context;
        }

        // Optimize Queries
        // Use AsNoTracking() for read-only queries to avoid EF Core tracking changes
        // var expenses = await _dbContext.Expenses.AsNoTracking().ToListAsync();

        public async Task<List<Expense>> GetAllAsync()
        {
            return await _context.Expenses.ToListAsync();
        }

        public async Task<List<Expense>> GetExpenses(int page, int pageSize)
        {
            return await _context.Expenses
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Expense?> GetByIdAsync(int id)
        {
            return await _context.Expenses.FindAsync(id);
        }

        public async Task AddAsync(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Expense expense)
        {
            _context.Expenses.Update(expense);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                await _context.SaveChangesAsync();
            }
        }
    }
}