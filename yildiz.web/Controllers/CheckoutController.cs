using Microsoft.AspNetCore.Mvc;
using yildiz.business.Abstract;
using yildiz.entities.Concrete;
using yildiz.web.Models;

namespace yildiz.web.Controllers;

public class CheckoutController : Controller
{
    private readonly IProductService _productService;
    private readonly IOrderService _orderService;

    public CheckoutController(
        IProductService productService,
        IOrderService orderService)
    {
        _productService = productService;
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart")
                   ?? new List<CartItem>();

        if (!cart.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

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
    public async Task<IActionResult> Complete(
        string customerName,
        string customerEmail,
        string address,
        string cardName,
        string cardNumber,
        string expiryDate,
        string cvv)
    {
        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart")
                   ?? new List<CartItem>();

        if (!cart.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        if (string.IsNullOrWhiteSpace(customerName) ||
            string.IsNullOrWhiteSpace(customerEmail) ||
            string.IsNullOrWhiteSpace(address))
        {
            ViewBag.Error = "Teslimat bilgilerini eksiksiz doldurun.";
            return await Index();
        }

        if (string.IsNullOrWhiteSpace(cardName) ||
            string.IsNullOrWhiteSpace(cardNumber) ||
            string.IsNullOrWhiteSpace(expiryDate) ||
            string.IsNullOrWhiteSpace(cvv))
        {
            ViewBag.Error = "Kart bilgilerini eksiksiz doldurun.";
            return await Index();
        }

        cardNumber = cardNumber.Replace(" ", "");

        if (cardNumber.Length != 16 ||
            !cardNumber.All(char.IsDigit))
        {
            ViewBag.Error = "Kart numarası 16 haneli olmalıdır.";
            return await Index();
        }

        if (cvv.Length != 3 ||
            !cvv.All(char.IsDigit))
        {
            ViewBag.Error = "CVV 3 haneli olmalıdır.";
            return await Index();
        }

        if (expiryDate.Length != 5 ||
            expiryDate[2] != '/')
        {
            ViewBag.Error = "Son kullanma tarihi AA/YY şeklinde olmalıdır.";
            return await Index();
        }

        var order = new Order
        {
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            Address = address,
            OrderDate = DateTime.Now
        };

        foreach (var item in cart)
        {
            var product = await _productService.GetByIdAsync(item.ProductId);

            if (product == null)
            {
                continue;
            }

            if (item.Quantity > product.Stock)
            {
                ViewBag.Error =
                    $"{product.Name} ürününden yeterli stok bulunmamaktadır.";

                return await Index();
            }

            order.OrderItems.Add(new OrderItem
            {
                ProductId = product.ProductId,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = item.Quantity
            });

            product.Stock -= item.Quantity;

            await _productService.UpdateAsync(product);
        }

        order.TotalPrice = order.OrderItems
            .Sum(x => x.Price * x.Quantity);

        await _orderService.AddAsync(order);

        HttpContext.Session.SetObject(
            "Cart",
            new List<CartItem>());

        return RedirectToAction(
            nameof(Success),
            new { id = order.OrderId });
    }

    [HttpGet]
    public async Task<IActionResult> Success(int id)
    {
        var order = await _orderService.GetByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}