using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using yildiz.DTO;
using yildiz.business.Abstract;
using yildiz.entities.Concrete;

namespace yildiz.web.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesApiController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesApiController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();

        var result = categories.Select(category => new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName
        });

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        var result = new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName
        };

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        var category = new Category
        {
            CategoryName = dto.CategoryName
        };

        await _categoryService.AddAsync(category);

        dto.CategoryId = category.CategoryId;

        return CreatedAtAction(
            nameof(GetById),
            new { id = category.CategoryId },
            dto);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        CategoryDto dto)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        category.CategoryName = dto.CategoryName;

        await _categoryService.UpdateAsync(category);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        await _categoryService.DeleteAsync(category.CategoryId); 

        return NoContent();
    }
}                
 
