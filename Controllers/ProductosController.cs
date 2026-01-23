using Microsoft.AspNetCore.Mvc;

namespace copilot_agentic_workflow_demo_v3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private static List<Producto> productos = new List<Producto>
        {
            new Producto(1, "Teclado mecánico", 89.99m, 15),
            new Producto(2, "Mouse inalámbrico", 39.50m, 30),
            new Producto(3, "Monitor 27\"", 249.00m, 8)
        };

        [HttpGet]
        public IActionResult GetProductos()
        {
            return Ok(productos);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetProducto(int id)
        {
            var producto = productos.FirstOrDefault(p => p.Id == id);
            return producto is null ? NotFound() : Ok(producto);
        }

        [HttpPost]
        public IActionResult CreateProducto([FromBody] ProductoCreate input)
        {
            var validation = Validar(input.Nombre, input.Precio, input.Stock);
            if (validation is not null) return validation;

            var nuevoId = productos.Any() ? productos.Max(p => p.Id) + 1 : 1;
            var producto = new Producto(nuevoId, input.Nombre.Trim(), input.Precio, input.Stock);
            productos.Add(producto);
            return Created($"/api/productos/{producto.Id}", producto);
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateProducto(int id, [FromBody] ProductoUpdate input)
        {
            var validation = Validar(input.Nombre, input.Precio, input.Stock);
            if (validation is not null) return validation;

            var producto = productos.FirstOrDefault(p => p.Id == id);
            if (producto is null) return NotFound();

            var actualizado = producto with
            {
                Nombre = input.Nombre.Trim(),
                Precio = input.Precio,
                Stock = input.Stock
            };

            var index = productos.FindIndex(p => p.Id == id);
            productos[index] = actualizado;
            return Ok(actualizado);
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteProducto(int id)
        {
            var removed = productos.RemoveAll(p => p.Id == id);
            return removed == 0 ? NotFound() : NoContent();
        }

        private IActionResult? Validar(string? nombre, decimal precio, int stock)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return BadRequest(new { error = "El nombre es obligatorio." });
            if (precio < 0) return BadRequest(new { error = "El precio no puede ser negativo." });
            if (stock < 0) return BadRequest(new { error = "El stock no puede ser negativo." });
            return null;
        }

        public record Producto(int Id, string Nombre, decimal Precio, int Stock);
        public record ProductoCreate(string Nombre, decimal Precio, int Stock);
        public record ProductoUpdate(string Nombre, decimal Precio, int Stock);
    }
}