using Microsoft.AspNetCore.Mvc;
using yildiz.business.Abstract;
using yildiz.web.Models;

namespace yildiz.web.Controllers;

public class CartController : Controller
{
    private readonly IProductService _productService;

    public CartController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int id)
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        {
            return RedirectToAction(
                "Login",
                "Account",
                new { returnUrl = Url.Action("Index", "Cart") }
            );
        }

        var product = await _productService.GetByIdAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart")
                   ?? new List<CartItem>();

        var existingItem = cart.FirstOrDefault(x => x.ProductId == id);

        if (existingItem != null)
        {
            if (existingItem.Quantity < product.Stock)
            {
                existingItem.Quantity++;
            }
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = product.ProductId,
                Quantity = 1
            });
        }

        HttpContext.Session.SetObject("Cart", cart);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart")
                   ?? new List<CartItem>();

        var products = new List<CartProduct>();

        foreach (var item in cart)
        {
            var product = await _productService.GetByIdAsync(item.ProductId);

            if (product != null)
            {
                products.Add(new CartProduct
                {
                    Product = product,
                    Quantity = item.Quantity
                });
            }
        }

        return View(products);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Increase(int id)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart")
                   ?? new List<CartItem>();

        var item = cart.FirstOrDefault(x => x.ProductId == id);
        var product = await _productService.GetByIdAsync(id);

        if (item != null && product != null && item.Quantity < product.Stock)
        {
            item.Quantity++;
        }

        HttpContext.Session.SetObject("Cart", cart);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Decrease(int id)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart")
                   ?? new List<CartItem>();

        var item = cart.FirstOrDefault(x => x.ProductId == id);

        if (item != null)
        {
            item.Quantity--;

            if (item.Quantity <= 0)
            {
                cart.Remove(item);
            }
        }

        HttpContext.Session.SetObject("Cart", cart);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int id)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart")
                   ?? new List<CartItem>();

        var item = cart.FirstOrDefault(x => x.ProductId == id);

        if (item != null)
        {
            cart.Remove(item);
        }

        HttpContext.Session.SetObject("Cart", cart);

        return RedirectToAction(nameof(Index));
    }
}