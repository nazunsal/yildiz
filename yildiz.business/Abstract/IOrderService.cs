using yildiz.entities.Concrete;

namespace yildiz.business.Abstract;

public interface IOrderService
{
    Task AddAsync(Order order);

    Task<Order?> GetByIdAsync(int id);

    Task<List<Order>> GetAllWithItemsAsync();

    Task UpdateAsync(Order order);
}