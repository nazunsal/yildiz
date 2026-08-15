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

    private bool IsAdmin()
    {
        return HttpContext.Session.GetString("IsAdmin") == "true";
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllAsync();

        return View(categories);
    }

    public async Task<IActionResult> Update(int id)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Admin");
        }

        var category = await _categoryService.GetByIdAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Category category)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Admin");
        }

        if (ModelState.IsValid)
        {
            await _categoryService.UpdateAsync(category);
            return RedirectToAction(nameof(Index));
        }

        return View(category);
    }

    public IActionResult Add()
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Admin");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(Category category)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Admin");
        }

        if (ModelState.IsValid)
        {
            await _categoryService.AddAsync(category);
            return RedirectToAction(nameof(Index));
        }

        return View(category);
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Admin");
        }

        await _categoryService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}