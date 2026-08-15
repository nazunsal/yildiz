using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yildiz.DataAccess.Context;

namespace yildiz.web.Controllers;

public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Login(string returnUrl = null)
    {
        if (HttpContext.Session.GetString("IsLoggedIn") == "true")
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        ViewBag.ReturnUrl = returnUrl;

        return View();
    }

    [HttpPost]
    public IActionResult Login(
        string username,
        string password,
        string returnUrl = null)
    {
        if (HttpContext.Session.GetString("IsLoggedIn") == "true")
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        if (username == "admin" && password == "1234")
        {
            HttpContext.Session.SetString("IsLoggedIn", "true");
            HttpContext.Session.SetString("Username", username);
            HttpContext.Session.SetString("IsAdmin", "true");
            HttpContext.Session.SetString("AdminUsername", username);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }

        ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
        ViewBag.ReturnUrl = returnUrl;

        return View();
    }

    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        {
            return RedirectToAction(nameof(Login));
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Orders()
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        {
            return RedirectToAction(nameof(Login));
        }

        var orders = await _context.Orders
            .Include(x => x.OrderItems)
            .OrderByDescending(x => x.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(
        int id,
        string status)
    {
        if (HttpContext.Session.GetString("IsLoggedIn") != "true")
        {
            return RedirectToAction(nameof(Login));
        }

        var order = await _context.Orders.FindAsync(id);

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

            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Orders));
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction(nameof(Login));
    }
}