using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Controllers;
using Api.Data;
using Api.Models;
using Xunit;

public class ProductsApiTests
{
    private readonly ProductDbContext _context;
    private readonly ProductsController _controller;

    public ProductsApiTests()
    {
        // Set up the in-memory database
        var options = new DbContextOptionsBuilder<ProductDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ProductDbContext(options);

        // Clear and seed the database
        ClearDatabase();
        SeedDatabase();

        _controller = new ProductsController(_context);
    }

    private void ClearDatabase()
    {
        _context.Products.RemoveRange(_context.Products);
        _context.SaveChanges();
    }

    private void SeedDatabase()
    {
        _context.Products.AddRange(
            new Product { Id = 1, Name = "Product1", Description = "", Price = 10, Stock = 100 },
            new Product { Id = 2, Name = "Product2", Description = "", Price = 20, Stock = 200 }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetProducts_ReturnsAllProducts()
    {
        // Act
        var result = await _controller.GetProducts();

        // Assert
        var actionResult = Assert.IsType<ActionResult<IEnumerable<Product>>>(result);
        var products = Assert.IsType<List<Product>>(actionResult.Value);
        Assert.Equal(2, products.Count);
    }

    [Fact]
    public async Task GetProduct_ReturnsProduct_WhenProductExists()
    {
        // Act
        var result = await _controller.GetProduct(1);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Product>>(result);
        var product = Assert.IsType<Product>(actionResult.Value);
        Assert.Equal(1, product.Id);
        Assert.Equal("Product1", product.Name);
    }

    [Fact]
    public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Act
        var result = await _controller.GetProduct(999);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateProduct_AddsProductAndReturnsCreatedAtAction()
    {
        // Arrange
        var newProduct = new Product { Id = 3, Name = "Product3", Description="", Price = 30, Stock = 300 };

        // Act
        var result = await _controller.CreateProduct(newProduct);

        // Assert
        var actionResult = Assert.IsType<ActionResult<Product>>(result);
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
        Assert.Equal("GetProduct", createdAtActionResult.ActionName);

        // Verify the product was added
        var productInDb = await _context.Products.FindAsync(3);
        Assert.NotNull(productInDb);
        Assert.Equal("Product3", productInDb.Name);
    }

    [Fact]
    public async Task UpdateProduct_UpdatesProductAndReturnsNoContent()
    {
        // Arrange
        var updatedProduct = new Product { Id = 1, Name = "UpdatedProduct1", Price = 15, Stock = 150 };

        // Act
        var result = await _controller.UpdateProduct(1, updatedProduct);

        // Assert
        Assert.IsType<NoContentResult>(result);

        // Verify the product was updated
        var productInDb = await _context.Products.FindAsync(1);
        Assert.NotNull(productInDb);
        Assert.Equal("UpdatedProduct1", productInDb.Name);
        Assert.Equal(15, productInDb.Price);
    }

    [Fact]
    public async Task UpdateProduct_ReturnsBadRequest_WhenIdMismatch()
    {
        // Arrange
        var updatedProduct = new Product { Id = 2, Name = "UpdatedProduct2", Price = 25, Stock = 250 };

        // Act
        var result = await _controller.UpdateProduct(1, updatedProduct);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_RemovesProductAndReturnsNoContent()
    {
        // Act
        var result = await _controller.DeleteProduct(1);

        // Assert
        Assert.IsType<NoContentResult>(result);

        // Verify the product was removed
        var productInDb = await _context.Products.FindAsync(1);
        Assert.Null(productInDb);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Act
        var result = await _controller.DeleteProduct(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}