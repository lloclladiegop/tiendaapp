using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaApp.API.Data;

namespace TiendaApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProductosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<List<Producto>> GetAll()
        => await _db.Productos.Where(p => p.Activo).OrderBy(p => p.Nombre).ToListAsync();

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] Producto p)
    {
        _db.Productos.Add(p);
        await _db.SaveChangesAsync();
        return Ok(p);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] Producto p)
    {
        var existing = await _db.Productos.FindAsync(id);
        if (existing is null) return NotFound();
        existing.Nombre = p.Nombre; existing.Categoria = p.Categoria;
        existing.Talla = p.Talla; existing.Color = p.Color;
        existing.PrecioCompra = p.PrecioCompra; existing.PrecioVenta = p.PrecioVenta;
        existing.Stock = p.Stock;
        await _db.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var p = await _db.Productos.FindAsync(id);
        if (p is null) return NotFound();
        p.Activo = false;
        await _db.SaveChangesAsync();
        return Ok();
    }
}