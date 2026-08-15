using yildiz.DataAccess.Abstract;
using yildiz.DataAccess.Context;
using yildiz.DataAccess.Repositories;
using yildiz.entities.Concrete;

namespace yildiz.DataAccess.Concrete;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context)
        : base(context)
    {
    }
}