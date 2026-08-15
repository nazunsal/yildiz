using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yildiz.DataAccess.Context;
using yildiz.entities.Concrete;

namespace yildiz.web.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Login(string returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(
        string username,
        string password,
        string returnUrl = null)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Username == username &&
                x.Password == password);

        if (user == null)
        {
            ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        HttpContext.Session.SetString(
            "IsLoggedIn",
            "true"
        );

        HttpContext.Session.SetString(
            "Username",
            user.Username
        );

        HttpContext.Session.SetString(
            "IsAdmin",
            "true"
        );

        HttpContext.Session.SetString(
            "AdminUsername",
            user.Username
        );

        if (!string.IsNullOrEmpty(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(
            "Index",
            "Home"
        );
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(
        string name,
        string username,
        string email,
        string password,
        string confirmPassword)
    {
        if (password != confirmPassword)
        {
            ViewBag.Error = "Şifreler eşleşmiyor.";

            return View();
        }

        var existingUsername = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Username == username);

        if (existingUsername != null)
        {
            ViewBag.Error =
                "Bu kullanıcı adı zaten kullanılıyor.";

            return View();
        }

        var existingEmail = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Email == email);

        if (existingEmail != null)
        {
            ViewBag.Error =
                "Bu e-posta adresi zaten kullanılıyor.";

            return View();
        }

        var user = new AppUser
        {
            Name = name,
            Username = username,
            Email = email,
            Password = password,
            IsAdmin = true
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        HttpContext.Session.SetString(
            "IsLoggedIn",
            "true"
        );

        HttpContext.Session.SetString(
            "Username",
            user.Username
        );

        HttpContext.Session.SetString(
            "IsAdmin",
            "true"
        );

        HttpContext.Session.SetString(
            "AdminUsername",
            user.Username
        );

        return RedirectToAction(
            "Index",
            "Home"
        );
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(
        string username,
        string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Username == username &&
                x.Email == email);

        if (user == null)
        {
            ViewBag.Error =
                "Kullanıcı adı veya e-posta bulunamadı.";

            return View();
        }

        HttpContext.Session.SetString(
            "ResetUserId",
            user.UserId.ToString()
        );

        return RedirectToAction(
            "ResetPassword"
        );
    }

    public IActionResult ResetPassword()
    {
        var resetUserId =
            HttpContext.Session.GetString(
                "ResetUserId"
            );

        if (string.IsNullOrEmpty(resetUserId))
        {
            return RedirectToAction(
                "ForgotPassword"
            );
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(
        string password,
        string confirmPassword)
    {
        if (password != confirmPassword)
        {
            ViewBag.Error =
                "Şifreler eşleşmiyor.";

            return View();
        }

        var resetUserId =
            HttpContext.Session.GetString(
                "ResetUserId"
            );

        if (!int.TryParse(
                resetUserId,
                out int userId))
        {
            return RedirectToAction(
                "ForgotPassword"
            );
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.UserId == userId);

        if (user == null)
        {
            return RedirectToAction(
                "ForgotPassword"
            );
        }

        user.Password = password;

        await _context.SaveChangesAsync();

        HttpContext.Session.Remove(
            "ResetUserId"
        );

        return RedirectToAction(
            "Login"
        );
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();

        return RedirectToAction(
            "Login"
        );
    }
}