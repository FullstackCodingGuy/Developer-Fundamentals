using ExpenseManager.Application.DTOs;
using ExpenseManager.Domain.Entities;
using ExpenseManager.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ExpenseManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Expense>>> GetAllExpenses()
        {
            return Ok(await _expenseService.GetAllExpensesAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpenseById(int id)
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            return expense != null ? Ok(expense) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult> AddExpense([FromBody] ExpenseDto expenseDto)
        {
            if (string.IsNullOrWhiteSpace(expenseDto.Title) || expenseDto.Amount <= 0)
                return BadRequest("Title and Amount are required fields.");

            await _expenseService.AddExpenseAsync(expenseDto);
            return CreatedAtAction(nameof(GetAllExpenses), new { });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateExpense(int id, [FromBody] ExpenseDto expenseDto)
        {
            await _expenseService.UpdateExpenseAsync(id, expenseDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteExpense(int id)
        {
            await _expenseService.DeleteExpenseAsync(id);
            return NoContent();
        }
    }
}