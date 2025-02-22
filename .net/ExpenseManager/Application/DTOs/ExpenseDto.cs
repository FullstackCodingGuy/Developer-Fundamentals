using System;
namespace ExpenseManager.Application.DTOs
{
    public class ExpenseDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}