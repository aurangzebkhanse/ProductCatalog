using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebApp.Models;
public class ProductsController : Controller
{
    private readonly IConfiguration _configuration;

    public ProductsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // List all products
    public async Task<IActionResult> Index()
    {
        var client = new HttpClient();
        var apiUrl = $"{_configuration["ApiBaseUrl"]}/products";

        var token = HttpContext.Session.GetString("JwtToken");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(apiUrl);
        if (response.IsSuccessStatusCode)
        {
            var products = await response.Content.ReadAsStringAsync();
            if (TempData["DeleteError"] != null)
            {
                ViewBag.Error = TempData["DeleteError"];
            }
            return View(JsonSerializer.Deserialize<List<ProductDTO>>(products));
        }

        return NotFound();
    }

    // Show the create product form
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // Handle product creation
    [HttpPost]
    public async Task<IActionResult> Create(ProductDTO product)
    {
        var client = new HttpClient();
        var apiUrl = $"{_configuration["ApiBaseUrl"]}/products";

        var token = HttpContext.Session.GetString("JwtToken");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }
        else if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
        {
            ViewBag.Error = "Unauthorized.";
        }
        else
        {
            ViewBag.Error = "Failed to create product.";
        }
        
        return View(product);
    }

    // Show the edit product form
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var client = new HttpClient();
        var apiUrl = $"{_configuration["ApiBaseUrl"]}/products/{id}";

        var token = HttpContext.Session.GetString("JwtToken");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(apiUrl);
        if (response.IsSuccessStatusCode)
        {
            var product = await response.Content.ReadAsStringAsync();
            return View(JsonSerializer.Deserialize<ProductDTO>(product));
        }
        else if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
        {
            ViewBag.Error = "Unauthorized.";
            return Unauthorized();
        }

        return NotFound();
    }

    // Handle product editing
    [HttpPost]
    public async Task<IActionResult> Edit(ProductDTO product)
    {
        var client = new HttpClient();
        var apiUrl = $"{_configuration["ApiBaseUrl"]}/products/{product.Id}";

        var token = HttpContext.Session.GetString("JwtToken");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var content = new StringContent(JsonSerializer.Serialize(product), Encoding.UTF8, "application/json");
        var response = await client.PutAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }
        else if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
        {
            ViewBag.Error = "Unauthorized.";
        }
        else
        {
            ViewBag.Error = "Failed to update product.";
        }
       
        return View(product);
    }

    // Handle product deletion
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var client = new HttpClient();
        var apiUrl = $"{_configuration["ApiBaseUrl"]}/products/{id}";

        var token = HttpContext.Session.GetString("JwtToken");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.DeleteAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("Index");
        }
        else if (response.StatusCode.Equals(HttpStatusCode.Unauthorized))
        {
            TempData["DeleteError"] = "Unauthorized.";
        }
        else
        {
            TempData["DeleteError"] = "Failed to delete product.";
        }

        return RedirectToAction("Index");
    }
}

