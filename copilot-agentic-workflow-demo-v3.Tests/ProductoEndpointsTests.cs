using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace copilot_agentic_workflow_demo_v3.Tests;

public class ProductoEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductoEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Productos_ReturnsOkWithList()
    {
        // Act
        var response = await _client.GetAsync("/api/productos");

        // Assert
        response.EnsureSuccessStatusCode();
        var productos = await response.Content.ReadFromJsonAsync<List<ProductoDto>>();
        Assert.NotNull(productos);
        Assert.NotEmpty(productos);
    }

    [Fact]
    public async Task Get_ProductoById_ExistingId_ReturnsOk()
    {
        // Arrange
        int existingId = 1;

        // Act
        var response = await _client.GetAsync($"/api/productos/{existingId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var producto = await response.Content.ReadFromJsonAsync<ProductoDto>();
        Assert.NotNull(producto);
        Assert.Equal(existingId, producto.Id);
    }

    [Fact]
    public async Task Get_ProductoById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        int nonExistingId = 9999;

        // Act
        var response = await _client.GetAsync($"/api/productos/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_Producto_ValidData_ReturnsCreated()
    {
        // Arrange
        var newProducto = new { Nombre = "Producto Test", Precio = 99.99m, Stock = 10 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/productos", newProducto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<ProductoDto>();
        Assert.NotNull(created);
        Assert.Equal(newProducto.Nombre, created.Nombre);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task Post_Producto_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var invalidProducto = new { Nombre = "", Precio = -10m, Stock = -5 };

        // Act
        var response = await _client.PostAsJsonAsync("/api/productos", invalidProducto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_Producto_ExistingId_ReturnsOk()
    {
        // Arrange
        int existingId = 1;
        var updateData = new { Nombre = "Producto Actualizado", Precio = 149.99m, Stock = 20 };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/productos/{existingId}", updateData);

        // Assert
        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<ProductoDto>();
        Assert.NotNull(updated);
        Assert.Equal(updateData.Nombre, updated.Nombre);
    }

    [Fact]
    public async Task Put_Producto_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        int nonExistingId = 9999;
        var updateData = new { Nombre = "Test", Precio = 10m, Stock = 5 };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/productos/{nonExistingId}", updateData);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Producto_ExistingId_ReturnsNoContent()
    {
        // Arrange - First create a product to delete
        var newProducto = new { Nombre = "Para Eliminar", Precio = 50m, Stock = 5 };
        var createResponse = await _client.PostAsJsonAsync("/api/productos", newProducto);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductoDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/productos/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_Producto_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        int nonExistingId = 9999;

        // Act
        var response = await _client.DeleteAsync($"/api/productos/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

// DTO para deserialización
public record ProductoDto(int Id, string Nombre, decimal Precio, int Stock);
