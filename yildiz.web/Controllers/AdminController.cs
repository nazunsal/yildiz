using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using yildiz.business.Abstract;

namespace yildiz.web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IOrderService _orderService;

    public AdminController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    public IActionResult Login(string returnUrl = null)
    {
        if (!string.IsNullOrEmpty(returnUrl))
        {
            return RedirectToAction(
                "Login",
                "Account",
                new { returnUrl });
        }

        return RedirectToAction("Login", "Account");
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Orders()
    {
        var orders = await _orderService.GetAllWithItemsAsync();

        return View(orders);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(
        int id,
        string status)
    {
        var order = await _orderService.GetByIdAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        var validStatuses = new[]
        {
            "Hazırlanıyor",
            "Kargoya Verildi",
            "Teslim Edildi",
            "İptal Edildi"
        };

        if (validStatuses.Contains(status))
        {
            order.Status = status;

            await _orderService.UpdateAsync(order);
        }

        return RedirectToAction(nameof(Orders));
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        HttpContext.Session.Clear();

        return RedirectToAction("Login", "Account");
    }
}