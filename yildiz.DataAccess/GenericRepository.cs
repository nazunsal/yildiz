using Microsoft.EntityFrameworkCore;
using yildiz.DataAccess.Abstract;
using yildiz.DataAccess.Context;
using yildiz.entities.Abstract;

namespace yildiz.DataAccess.Repositories;

public class GenericRepository<T> : IGenericRepository<T>
    where T : class, IEntity, new()
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _table;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _table = _context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _table.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _table.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _table.AddAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        _table.Update(entity);
    }

    public async Task DeleteAsync(T entity)
    {
        _table.Remove(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}