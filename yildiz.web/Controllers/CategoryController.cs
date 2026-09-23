using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using yildiz.business.Abstract;
using yildiz.entities.Concrete;

namespace yildiz.web.Controllers;

public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();

        return View(categories);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Category category)
    {
        if (ModelState.IsValid)
        {
            await _categoryService.UpdateAsync(category);

            return RedirectToAction(nameof(Index));
        }

        return View(category);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Category category)
    {
        if (ModelState.IsValid)
        {
            await _categoryService.AddAsync(category);

            return RedirectToAction(nameof(Index));
        }

        return View(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }
}