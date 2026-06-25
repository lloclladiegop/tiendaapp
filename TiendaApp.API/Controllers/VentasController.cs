using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiendaApp.API.Data;

namespace TiendaApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VentasController : ControllerBase
{
    private readonly AppDbContext _db;
    public VentasController(AppDbContext db) => _db = db;

    [HttpGet("resumen")]
    public async Task<IActionResult> Resumen()
    {
        var hoy = DateTime.Today;
        var ventas = await _db.Ventas
            .Where(v => v.Fecha.Date == hoy)
            .OrderByDescending(v => v.Fecha)
            .ToListAsync();

        return Ok(new
        {
            TotalVentas = ventas.Sum(v => v.Total),
            NumeroVentas = ventas.Count,
            TotalEfectivo = ventas.Where(v => v.MetodoPago == "Efectivo").Sum(v => v.Total),
            TotalDigital = ventas.Where(v => v.MetodoPago != "Efectivo").Sum(v => v.Total),
            UltimasVentas = ventas.Take(10).Select(v => new
            {
                v.Id, v.Fecha, v.ClienteNombre, v.MetodoPago, v.Total
            })
        });
    }

    [HttpPost]
    public async Task<IActionResult> Registrar([FromBody] NuevaVentaDto dto)
    {
        var venta = new Venta
        {
            ClienteNombre = string.IsNullOrEmpty(dto.ClienteNombre) ? "Cliente general" : dto.ClienteNombre,
            MetodoPago = dto.MetodoPago,
            Total = dto.Total
        };

        foreach (var item in dto.Items)
        {
            var producto = await _db.Productos.FindAsync(item.ProductoId);
            if (producto is null) return BadRequest($"Producto {item.ProductoId} no encontrado");
            if (producto.Stock < item.Cantidad) return BadRequest($"Stock insuficiente para {producto.Nombre}");

            producto.Stock -= item.Cantidad;
            venta.Detalles.Add(new VentaDetalle
            {
                ProductoNombre = producto.Nombre,
                Talla = producto.Talla,
                Color = producto.Color,
                Cantidad = item.Cantidad,
                PrecioUnitario = item.PrecioUnitario
            });
        }

        _db.Ventas.Add(venta);
        await _db.SaveChangesAsync();
        return Ok(new { venta.Id, venta.Total });
    }
}

public class NuevaVentaDto
{
    public string ClienteNombre { get; set; } = "";
    public string MetodoPago { get; set; } = "Efectivo";
    public decimal Total { get; set; }
    public List<ItemVentaDto> Items { get; set; } = new();
}

public class ItemVentaDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}