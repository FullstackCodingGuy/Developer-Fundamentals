using ExpenseManager.Application.DTOs;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace ExpenseManager.Application.Interfaces
{
    public interface IExpenseService
    {
        Task<IEnumerable<ExpenseDto>> GetExpensesAsync();
        Task<ExpenseDto> GetExpenseByIdAsync(int id);
        Task<ExpenseDto> CreateExpenseAsync(ExpenseDto expenseDto);
        Task<bool> UpdateExpenseAsync(int id, ExpenseDto expenseDto);
        Task<bool> DeleteExpenseAsync(int id);
    }
}