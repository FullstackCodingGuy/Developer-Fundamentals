using ExpenseManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
public class CategoryRepositoryCachingDecorator : ICategoryRepository
{
    private readonly ICategoryRepository _inner;
    private readonly IMemoryCache _cache;

    private const string CacheKey = "categories_cache";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public CategoryRepositoryCachingDecorator(ICategoryRepository inner, IMemoryCache cache)
    {
        _inner = inner;
        _cache = cache;
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        Console.WriteLine("CategoryRepositoryCachingDecorator.GetAllSync");

        return await _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheDuration;
            return await _inner.GetAllAsync();
        });
    }

    public Task<Category> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Category category)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Category category)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}
