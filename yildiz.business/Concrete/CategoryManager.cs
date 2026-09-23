using yildiz.business.Abstract;
using yildiz.DataAccess.Abstract;
using yildiz.entities.Concrete;

namespace yildiz.business.Concrete;

public class CategoryManager : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryManager(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task AddAsync(Category category)
    {
        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        await _categoryRepository.UpdateAsync(category);
        await _categoryRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category != null)
        {
            await _categoryRepository.DeleteAsync(category);
            await _categoryRepository.SaveChangesAsync();
        }
    }
}