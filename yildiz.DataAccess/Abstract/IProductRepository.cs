using yildiz.entities.Concrete;

namespace yildiz.DataAccess.Abstract;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<List<Product>> GetProductsWithCategoryAsync();
}