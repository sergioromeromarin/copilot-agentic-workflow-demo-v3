var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5500")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

var productos = new List<Producto>
{
    new Producto(1, "Teclado mecánico", 89.99m, 15),
    new Producto(2, "Mouse inalámbrico", 39.50m, 30),
    new Producto(3, "Monitor 27\"", 249.00m, 8)
};

app.MapGet("/", () => Results.Ok(new { message = "API CRUD de Productos con datos en memoria" }))
    .WithName("Root")
    .WithOpenApi();

app.MapGet("/api/productos", () => Results.Ok(productos))
    .WithName("ListarProductos")
    .WithOpenApi();

app.MapGet("/api/productos/{id:int}", (int id) =>
{
    var producto = productos.FirstOrDefault(p => p.Id == id);
    return producto is null
        ? Results.NotFound()
        : Results.Ok(producto);
})
.WithName("ObtenerProducto")
.WithOpenApi();

app.MapPost("/api/productos", (ProductoCreate input) =>
{
    var validation = Validar(input.Nombre, input.Precio, input.Stock);
    if (validation is not null) return validation;

    var nuevoId = productos.Any() ? productos.Max(p => p.Id) + 1 : 1;
    var producto = new Producto(nuevoId, input.Nombre.Trim(), input.Precio, input.Stock);
    productos.Add(producto);
    return Results.Created($"/api/productos/{producto.Id}", producto);
})
.WithName("CrearProducto")
.WithOpenApi();

app.MapPut("/api/productos/{id:int}", (int id, ProductoUpdate input) =>
{
    var validation = Validar(input.Nombre, input.Precio, input.Stock);
    if (validation is not null) return validation;

    var producto = productos.FirstOrDefault(p => p.Id == id);
    if (producto is null) return Results.NotFound();

    var actualizado = producto with
    {
        Nombre = input.Nombre.Trim(),
        Precio = input.Precio,
        Stock = input.Stock
    };

    var index = productos.FindIndex(p => p.Id == id);
    productos[index] = actualizado;
    return Results.Ok(actualizado);
})
.WithName("ActualizarProducto")
.WithOpenApi();

app.MapDelete("/api/productos/{id:int}", (int id) =>
{
    var removed = productos.RemoveAll(p => p.Id == id);
    return removed == 0
        ? Results.NotFound()
        : Results.NoContent();
})
.WithName("EliminarProducto")
.WithOpenApi();

app.Run();

static IResult? Validar(string? nombre, decimal precio, int stock)
{
    if (string.IsNullOrWhiteSpace(nombre)) return Results.BadRequest(new { error = "El nombre es obligatorio." });
    if (precio < 0) return Results.BadRequest(new { error = "El precio no puede ser negativo." });
    if (stock < 0) return Results.BadRequest(new { error = "El stock no puede ser negativo." });
    return null;
}

record Producto(int Id, string Nombre, decimal Precio, int Stock);
record ProductoCreate(string Nombre, decimal Precio, int Stock);
record ProductoUpdate(string Nombre, decimal Precio, int Stock);

// Make Program class accessible to tests
public partial class Program { }
