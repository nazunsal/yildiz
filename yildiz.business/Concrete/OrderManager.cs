using yildiz.business.Abstract;
using yildiz.DataAccess.Abstract;
using yildiz.entities.Concrete;

namespace yildiz.business.Concrete;

public class OrderManager : IOrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderManager(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task AddAsync(Order order)
    {
        await _orderRepository.AddAsync(order);
        await _orderRepository.SaveChangesAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _orderRepository.GetByIdWithItemsAsync(id);
    }

    public async Task<List<Order>> GetAllWithItemsAsync()
    {
        return await _orderRepository.GetAllWithItemsAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        await _orderRepository.UpdateAsync(order);
        await _orderRepository.SaveChangesAsync();
    }
}