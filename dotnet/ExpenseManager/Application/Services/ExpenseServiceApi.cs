using ExpenseManager.Application.DTOs;
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
namespace ExpenseManager.Application.Services
{
    public class ExpenseServiceApi : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;

        public ExpenseServiceApi(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        public Task AddExpenseAsync(ExpenseDto expenseDto)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteExpenseAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Expense>> GetAllExpensesAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task<Expense> GetExpenseByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateExpenseAsync(int id, ExpenseDto expenseDto)
        {
            throw new System.NotImplementedException();
        }
    }
}