using ExpenseManager.Application.Interfaces;
using ExpenseManager.Domain.Entities;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.Linq;
namespace ExpenseManager.Infrastructure.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly List<Expense> _expenses = new();

        public async Task<List<Expense>> GetAllAsync() => await Task.FromResult(_expenses);

        public async Task<Expense?> GetByIdAsync(int id) => await Task.FromResult(_expenses.FirstOrDefault(e => e.Id == id));

        public async Task AddAsync(Expense expense)
        {
            expense.Id = _expenses.Count + 1;
            _expenses.Add(expense);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(Expense expense)
        {
            var existingExpense = _expenses.FirstOrDefault(e => e.Id == expense.Id);
            if (existingExpense != null)
            {
                existingExpense.Title = expense.Title;
                existingExpense.Description = expense.Description;
                existingExpense.Date = expense.Date;
                existingExpense.Amount = expense.Amount;
                existingExpense.Category = expense.Category;
            }
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var expense = _expenses.FirstOrDefault(e => e.Id == id);
            if (expense != null)
            {
                _expenses.Remove(expense);
            }
            await Task.CompletedTask;
        }
    }
}