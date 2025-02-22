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

        public Task<IEnumerable<Expense>> GetAllAsync()
        {
            return Task.FromResult(_expenses.AsEnumerable());
        }

        public Task<Expense> GetByIdAsync(int id)
        {
            var expense = _expenses.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(expense);
        }

        public Task AddAsync(Expense expense)
        {
            expense.Id = _expenses.Any() ? _expenses.Max(e => e.Id) + 1 : 1;
            _expenses.Add(expense);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Expense expense)
        {
            var index = _expenses.FindIndex(e => e.Id == expense.Id);
            if (index >= 0)
                _expenses[index] = expense;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Expense expense)
        {
            _expenses.Remove(expense);
            return Task.CompletedTask;
        }
    }
}