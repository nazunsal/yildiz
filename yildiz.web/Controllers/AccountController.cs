using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using yildiz.business.Abstract;
using yildiz.entities.Concrete;
using yildiz.web.Services;

namespace yildiz.web.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly EmailService _emailService;

    public AccountController(
        IUserService userService,
        IPasswordResetTokenService passwordResetTokenService,
        EmailService emailService)
    {
        _userService = userService;
        _passwordResetTokenService = passwordResetTokenService;
        _emailService = emailService;
    }

    public IActionResult Login(string returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        string username,
        string password,
        string returnUrl = null)
    {
        var user = await _userService.GetByUsernameAsync(username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            ViewBag.Error = "Kullanıcı adı veya şifre hatalı.";
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(
                ClaimTypes.NameIdentifier,
                user.UserId.ToString()),
            new Claim(
                ClaimTypes.Role,
                user.IsAdmin ? "Admin" : "User")
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        HttpContext.Session.SetString("IsLoggedIn", "true");
        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetString(
            "IsAdmin",
            user.IsAdmin.ToString().ToLower());

        if (user.IsAdmin)
        {
            HttpContext.Session.SetString(
                "AdminUsername",
                user.Username);
        }
        else
        {
            HttpContext.Session.Remove("AdminUsername");
        }

        if (!string.IsNullOrEmpty(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(
        string name,
        string username,
        string email,
        string password,
        string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(password) ||
            password.Length < 6)
        {
            ViewBag.Error =
                "Şifre en az 6 karakter olmalıdır.";

            return View();
        }

        if (password != confirmPassword)
        {
            ViewBag.Error = "Şifreler eşleşmiyor.";
            return View();
        }

        var existingUsername =
            await _userService.GetByUsernameAsync(username);

        if (existingUsername != null)
        {
            ViewBag.Error =
                "Bu kullanıcı adı zaten kullanılıyor.";

            return View();
        }

        var existingEmail =
            await _userService.GetByEmailAsync(email);

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
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            IsAdmin = false
        };

        await _userService.AddAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(
                ClaimTypes.NameIdentifier,
                user.UserId.ToString()),
            new Claim(ClaimTypes.Role, "User")
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        HttpContext.Session.SetString("IsLoggedIn", "true");
        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetString("IsAdmin", "false");
        HttpContext.Session.Remove("AdminUsername");

        return RedirectToAction("Index", "Home");
    }

    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(
        string username,
        string email)
    {
        var user =
            await _userService.GetByUsernameAndEmailAsync(
                username,
                email);

        if (user == null)
        {
            ViewBag.Error =
                "Kullanıcı adı veya e-posta bulunamadı.";

            return View();
        }

        var tokenBytes =
            RandomNumberGenerator.GetBytes(32);

        var token = Convert.ToBase64String(tokenBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

        using var sha256 = SHA256.Create();

        var tokenHashBytes =
            sha256.ComputeHash(
                Encoding.UTF8.GetBytes(token));

        var tokenHash =
            Convert.ToHexString(tokenHashBytes);

        var resetToken = new PasswordResetToken
        {
            UserId = user.UserId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            IsUsed = false
        };

        await _passwordResetTokenService.CreateAsync(resetToken);

        var resetLink =
            Url.Action(
                "ResetPassword",
                "Account",
                new { token },
                Request.Scheme);

        if (string.IsNullOrEmpty(resetLink))
        {
            ViewBag.Error =
                "Şifre sıfırlama bağlantısı oluşturulamadı.";

            return View();
        }

        try
        {
            await _emailService.SendPasswordResetEmailAsync(
                user.Email,
                user.Username,
                resetLink);
        }
        catch
        {
            ViewBag.Error =
                "Şifre sıfırlama e-postası gönderilemedi.";

            return View();
        }

        ViewBag.Success =
            "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.";

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return RedirectToAction("ForgotPassword");
        }

        using var sha256 = SHA256.Create();

        var tokenHashBytes =
            sha256.ComputeHash(
                Encoding.UTF8.GetBytes(token));

        var tokenHash =
            Convert.ToHexString(tokenHashBytes);

        var resetToken =
            await _passwordResetTokenService
                .GetByTokenHashAsync(tokenHash);

        if (resetToken == null ||
            resetToken.IsUsed ||
            resetToken.ExpiresAt <= DateTime.UtcNow)
        {
            ViewBag.Error =
                "Şifre sıfırlama bağlantısı geçersiz veya süresi dolmuş.";

            return View();
        }

        ViewBag.Token = token;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(
        string token,
        string password,
        string confirmPassword)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return RedirectToAction("ForgotPassword");
        }

        if (string.IsNullOrWhiteSpace(password) ||
            password.Length < 6)
        {
            ViewBag.Error =
                "Şifre en az 6 karakter olmalıdır.";

            ViewBag.Token = token;

            return View();
        }

        if (password != confirmPassword)
        {
            ViewBag.Error = "Şifreler eşleşmiyor.";
            ViewBag.Token = token;

            return View();
        }

        using var sha256 = SHA256.Create();

        var tokenHashBytes =
            sha256.ComputeHash(
                Encoding.UTF8.GetBytes(token));

        var tokenHash =
            Convert.ToHexString(tokenHashBytes);

        var resetToken =
            await _passwordResetTokenService
                .GetByTokenHashAsync(tokenHash);

        if (resetToken == null ||
            resetToken.IsUsed ||
            resetToken.ExpiresAt <= DateTime.UtcNow)
        {
            ViewBag.Error =
                "Şifre sıfırlama bağlantısı geçersiz veya süresi dolmuş.";

            return View();
        }

        if (resetToken.User == null)
        {
            return RedirectToAction("ForgotPassword");
        }

        resetToken.User.Password =
            BCrypt.Net.BCrypt.HashPassword(password);

        await _userService.UpdateAsync(resetToken.User);

        resetToken.IsUsed = true;

        await _passwordResetTokenService.UpdateAsync(resetToken);

        return RedirectToAction("Login");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        HttpContext.Session.Clear();

        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}