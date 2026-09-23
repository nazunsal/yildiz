using Microsoft.EntityFrameworkCore;
using yildiz.DataAccess.Abstract;
using yildiz.DataAccess.Context;
using yildiz.DataAccess.Repositories;
using yildiz.entities.Concrete;

namespace yildiz.DataAccess.Concrete;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetAllWithItemsAsync()
    {
        return await _context.Orders
            .Include(x => x.OrderItems)
            .OrderByDescending(x => x.OrderDate)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdWithItemsAsync(int id)
    {
        return await _context.Orders
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync(x => x.OrderId == id);
    }
}