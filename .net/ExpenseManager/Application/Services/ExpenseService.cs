using ExpenseManager.Application.DTOs;
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
namespace ExpenseManager.Application.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;

        public ExpenseService(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        public async Task<List<Expense>> GetAllExpensesAsync() => await _expenseRepository.GetAllAsync();

        public async Task<Expense?> GetExpenseByIdAsync(int id) => await _expenseRepository.GetByIdAsync(id);

        public async Task AddExpenseAsync(ExpenseDto expenseDto)
        {
            var expense = new Expense
            {
                Title = expenseDto.Title,
                Description = expenseDto.Description,
                Date = expenseDto.Date,
                Amount = expenseDto.Amount,
                Category = expenseDto.Category
            };
            await _expenseRepository.AddAsync(expense);
        }

        public async Task UpdateExpenseAsync(int id, ExpenseDto expenseDto)
        {
            var existingExpense = await _expenseRepository.GetByIdAsync(id);
            if (existingExpense != null)
            {
                existingExpense.Title = expenseDto.Title;
                existingExpense.Description = expenseDto.Description;
                existingExpense.Date = expenseDto.Date;
                existingExpense.Amount = expenseDto.Amount;
                existingExpense.Category = expenseDto.Category;

                await _expenseRepository.UpdateAsync(existingExpense);
            }
        }

        public async Task DeleteExpenseAsync(int id) => await _expenseRepository.DeleteAsync(id);
    }
}