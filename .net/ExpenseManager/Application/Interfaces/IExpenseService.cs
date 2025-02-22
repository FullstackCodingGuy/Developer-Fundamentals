using ExpenseManager.Application.DTOs;
using ExpenseManager.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace ExpenseManager.Application.Interfaces
{
    public interface IExpenseService
    {
        Task<List<Expense>> GetAllExpensesAsync();
        Task<Expense?> GetExpenseByIdAsync(int id);
        Task AddExpenseAsync(ExpenseDto expenseDto);
        Task UpdateExpenseAsync(int id, ExpenseDto expenseDto);
        Task DeleteExpenseAsync(int id);
    }
}