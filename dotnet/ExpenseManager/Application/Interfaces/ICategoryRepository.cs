using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseManager.Domain.Entities;
public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllAsync();
}
