using Microsoft.EntityFrameworkCore;

namespace TiendaApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<VentaDetalle> VentaDetalles => Set<VentaDetalle>();
}

public class Producto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Categoria { get; set; } = "";
    public string Talla { get; set; } = "";
    public string Color { get; set; } = "";
    public decimal PrecioCompra { get; set; }
    public decimal PrecioVenta { get; set; }
    public int Stock { get; set; }
    public bool Activo { get; set; } = true;
}

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Telefono { get; set; } = "";
    public string DNI { get; set; } = "";
    public string Email { get; set; } = "";
    public string Notas { get; set; } = "";
    public DateTime CreadoEn { get; set; } = DateTime.Now;
    public List<Venta> Ventas { get; set; } = new();
}

public class Venta
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public int? ClienteId { get; set; }
    public string ClienteNombre { get; set; } = "Cliente general";
    public string MetodoPago { get; set; } = "Efectivo";
    public decimal Total { get; set; }
    public Cliente? Cliente { get; set; }
    public List<VentaDetalle> Detalles { get; set; } = new();
}

public class VentaDetalle
{
    public int Id { get; set; }
    public int VentaId { get; set; }
    public string ProductoNombre { get; set; } = "";
    public string Talla { get; set; } = "";
    public string Color { get; set; } = "";
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public Venta? Venta { get; set; }
}