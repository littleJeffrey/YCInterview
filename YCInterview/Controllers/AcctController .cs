using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using YCInterview.Models;

namespace YCInterview.Controllers;

public class AcctController : Controller
{
    private readonly TodoDbContext _context;

    public AcctController(TodoDbContext context)
    {
        _context = context;
    }

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        byte[] pwBytes = Encoding.ASCII.GetBytes(password + "_example");
        pwBytes = SHA256.HashData(pwBytes);
        var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == pwBytes);
        if (user == null)
        {
            ViewBag.Error = "帳號或密碼錯誤";
            return View();
        }

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.Id.ToString())
            };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Todo");
    }

    public IActionResult Register() => View();

    [HttpPost]
    public IActionResult Register(string username, string password)
    {
        if (_context.Users.Any(u => u.Username == username))
        {
            ViewBag.Error = "使用者名稱已存在";
            return View();
        }

        byte[] pwBytes = Encoding.ASCII.GetBytes(password + "_example");
        pwBytes = SHA256.HashData(pwBytes);
        var user = new User { Username = username, Password = pwBytes };
        _context.Users.Add(user);
        _context.SaveChanges();

        return RedirectToAction("Login");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}