using Microsoft.AspNetCore.Mvc;
using yildiz.business.Abstract;
using yildiz.entities.Concrete;

namespace yildiz.web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductController(
        IProductService productService,
        ICategoryService categoryService)
    {
        _productService = productService;
        _categoryService = categoryService;
    }

    private bool IsAdmin()
    {
        return HttpContext.Session.GetString("IsAdmin") == "true";
    }

    public async Task<IActionResult> Index(string search)
    {
        var values = await _productService.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            values = values
                .Where(x =>
                    x.Name.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase
                    )
                    ||
                    (x.Description != null &&
                     x.Description.Contains(
                         search,
                         StringComparison.OrdinalIgnoreCase
                     ))
                )
                .ToList();
        }

        ViewBag.Search = search;

        return View(values);
    }

    public async Task<IActionResult> Add()
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Admin");
        }

        var categories = await _categoryService.GetAllAsync();

        ViewBag.Categories = categories;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Product product)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Admin");
        }

        if (ModelState.IsValid)
        {
            await _productService.AddAsync(product);

            return RedirectToAction(nameof(Index));
        }

        var categories = await _categoryService.GetAllAsync();

        ViewBag.Categories = categories;

        return View(product);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    public async Task<IActionResult> Update(int id)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Admin");
        }

        var product = await _productService.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        var categories = await _categoryService.GetAllAsync();

        ViewBag.Categories = categories;

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Product product)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Admin");
        }

        if (ModelState.IsValid)
        {
            await _productService.UpdateAsync(product);

            return RedirectToAction(nameof(Index));
        }

        var categories = await _categoryService.GetAllAsync();

        ViewBag.Categories = categories;

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAdmin())
        {
            return RedirectToAction("Login", "Admin");
        }

        await _productService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }
}