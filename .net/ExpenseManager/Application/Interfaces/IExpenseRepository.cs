using ExpenseManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ExpenseManager.Application.Interfaces
{
    public interface IExpenseRepository
    {
        Task<List<Expense>> GetAllAsync();
        Task<Expense?> GetByIdAsync(int id);
        Task AddAsync(Expense expense);
        Task UpdateAsync(Expense expense);
        Task DeleteAsync(int id);
    }
}