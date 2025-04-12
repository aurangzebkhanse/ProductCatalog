using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class AuthController : Controller
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        var client = new HttpClient();
        var apiUrl = $"{_configuration["ApiBaseUrl"]}/auth/login";

        var payload = new { Username = username, Password = password };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadAsStringAsync();
            HttpContext.Session.SetString("JwtToken", token);
            return RedirectToAction("Index", "Products");
        }

        ViewBag.Error = "Invalid username or password.";
        return View();
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Remove("JwtToken");
        return RedirectToAction("Login");
    }
}

