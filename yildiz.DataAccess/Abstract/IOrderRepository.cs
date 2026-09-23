using yildiz.entities.Concrete;

namespace yildiz.DataAccess.Abstract;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<List<Order>> GetAllWithItemsAsync();

    Task<Order?> GetByIdWithItemsAsync(int id);
}