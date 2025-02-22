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

        public async Task<IEnumerable<ExpenseDto>> GetExpensesAsync()
        {
            var expenses = await _expenseRepository.GetAllAsync();
            return expenses.Select(e => new ExpenseDto
            {
                Id = e.Id,
                Amount = e.Amount,
                Description = e.Description,
                Date = e.Date,
                Category = e.Category
            });
        }

        public async Task<ExpenseDto> GetExpenseByIdAsync(int id)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense == null) return null;

            return new ExpenseDto
            {
                Id = expense.Id,
                Amount = expense.Amount,
                Description = expense.Description,
                Date = expense.Date,
                Category = expense.Category
            };
        }

        public async Task<ExpenseDto> CreateExpenseAsync(ExpenseDto expenseDto)
        {
            var expense = new Expense
            {
                Amount = expenseDto.Amount,
                Description = expenseDto.Description,
                Date = expenseDto.Date,
                Category = expenseDto.Category
            };

            await _expenseRepository.AddAsync(expense);
            expenseDto.Id = expense.Id;
            return expenseDto;
        }

        public async Task<bool> UpdateExpenseAsync(int id, ExpenseDto expenseDto)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense == null)
                return false;

            expense.Amount = expenseDto.Amount;
            expense.Description = expenseDto.Description;
            expense.Date = expenseDto.Date;
            expense.Category = expenseDto.Category;

            await _expenseRepository.UpdateAsync(expense);
            return true;
        }

        public async Task<bool> DeleteExpenseAsync(int id)
        {
            var expense = await _expenseRepository.GetByIdAsync(id);
            if (expense == null)
                return false;

            await _expenseRepository.DeleteAsync(expense);
            return true;
        }
    }
}