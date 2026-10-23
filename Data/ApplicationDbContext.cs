using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockWeb.Models;

namespace StockWeb.Data
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
        public DbSet<Remito> Remitos { get; set; }
        public DbSet<DetalleRemito> DetallesRemito { get; set; }

    }
}
