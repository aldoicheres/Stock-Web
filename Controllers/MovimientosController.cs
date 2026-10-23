using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockWeb.Data;
using StockWeb.Models;

namespace StockWeb.Controllers
{
    public class MovimientosController : Controller
    {
        private readonly StockDbContext _context;

        public MovimientosController(StockDbContext context)
        {
            _context = context;
        }

        // GET: Movimientos
        public async Task<IActionResult> Index()
        {
            var movimientos = await _context.Movimientos
                .Include(m => m.Producto)
                .OrderByDescending(m => m.FechaRetiro)
                .ToListAsync();

            return View(movimientos);
        }

        [HttpGet]
        public async Task<IActionResult> Crear(string? buscar)
        {
            var query = _context.Productos.AsQueryable();

            if (!string.IsNullOrEmpty(buscar))
            {
                query = query.Where(p => p.Nombre.Contains(buscar) || p.Descripcion.Contains(buscar));
                ViewBag.Buscar = buscar;
            }

            var productos = await query.ToListAsync();

            ViewBag.ProductosId = new SelectList(
                productos.Select(p => new { Id = p.Id, Nombre = $"{p.Nombre} (Stock Actual: {p.StockActual})" }),
                "Id",
                "Nombre"
            );

            return View(new RemitoViewModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(RemitoViewModel model)
        {
            if (model.Items == null || !model.Items.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un producto al remito.");
                await CargarProductosViewBag();
                return View(model);
            }

            // Usamos una transacción para garantizar que si falla un producto, no se guarde nada
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Crear la Cabecera del Remito
                var remito = new Remito
                {
                    Fecha = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    TipoMovimiento = model.TipoMovimiento,
                    PersonaRetira = model.PersonaRetira,
                    Observaciones = model.Observaciones,
                    UsuarioId = int.Parse(HttpContext.Session.GetString("USUARIO_ID") ?? "1"),
                    UltimaModificacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                _context.Remitos.Add(remito);
                await _context.SaveChangesAsync(); // Genera el Remito.Id

                // 2. Procesar cada ítem / artículo enviado
                foreach (var item in model.Items)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoId);

                    if (producto == null)
                    {
                        throw new Exception($"El producto con ID {item.ProductoId} no existe.");
                    }

                    // Validar stock si es Egreso
                    if (model.TipoMovimiento == "Egreso" && producto.StockActual < item.Cantidad)
                    {
                        throw new Exception($"Stock insuficiente para {producto.Nombre}. Disponible: {producto.StockActual}");
                    }

                    // Impactar stock

                    
                    if (model.TipoMovimiento == "Ingreso")
                    {
                        producto.StockActual += item.Cantidad;

                    }
                    else if (model.TipoMovimiento == "Egreso")
                    {
                        producto.StockActual -= item.Cantidad;

                    }
                    producto.UsuarioId = HttpContext.Session.GetString("USUARIO_ID") ?? "1";
                    producto.UltimaModificacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");



                    // Crear el detalle
                    var detalle = new DetalleRemito
                    {
                        RemitoId = remito.Id,
                        ProductosId = item.ProductoId,
                        Cantidad = Convert.ToDouble(item.Cantidad),
                        UsuarioId = long.Parse(HttpContext.Session.GetString("USUARIO_ID") ?? "1"),
                        UltimaModificacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    };

                    _context.DetallesRemito.Add(detalle);
                    _context.Productos.Update(producto);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); // Confirmar cambios en SQLite

                return RedirectToAction(nameof(Remito), new { id = remito.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError("", ex.Message);
                await CargarProductosViewBag();
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Remito(long id)
        {
            var remito = await _context.Remitos
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (remito == null)
            {
                return NotFound();
            }

            return View(remito);
        }

        private async Task CargarProductosViewBag(long? seleccionadoId = null)
        {
            // 1. Obtener la lista de productos de la base de datos
            var productos = await _context.Productos.ToListAsync();

            // 2. Mapear los datos para mostrar Nombre y Stock en el texto de la opción
            ViewBag.ProductosId = new SelectList(
                productos.Select(p => new
                {
                    Id = p.Id,
                    NombreCompleto = $"{p.Nombre} (Stock disponible: {p.StockActual})"
                }),
                "Id",             // Propiedad para el valor enviado (value)
                "NombreCompleto", // Propiedad para el texto visible (text)
                seleccionadoId    // ID del producto seleccionado por defecto (opcional)
            );
        }
    }
}