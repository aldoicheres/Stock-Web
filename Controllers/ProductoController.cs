using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StockWeb.Data;
using StockWeb.Models;
namespace StockWeb.Controllers
   {
    [Authorize(Roles = "1")]
    public class ProductoController : Controller
    {
        private readonly StockDbContext _context;

        // Inyección de dependencias del DbContext
        public ProductoController(StockDbContext context)
        {
            _context = context;
        }

        // GET: ProductoController
        [HttpGet("/Producto/ProductosLista")]
        public async Task<IActionResult> ProductosLista()
        {
            // Traemos la lista de productos incluyendo su categoría relacionada
            var productos = await _context.Productos
                .Include(p => p.Categoria)
                .ToListAsync();

            return View(productos);
        }

                // GET: ProductoController/Create
        public IActionResult Crear()
        {
            // Cargar combo desplegable de categorías para la vista
            ViewBag.CategoriasId = new SelectList(_context.Categorias, "Id", "Nombre");
            return View();
        }

        // POST: ProductoController/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Producto producto)
        {
            if (ModelState.IsValid)
            {
                producto.UltimaModificacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                producto.UsuarioId = HttpContext.Session.GetString("USUARIO_ID") ?? "SYSTEM";
                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ProductosLista));
            }

            // Si hay error de validación, recargamos el combo de categorías
            ViewBag.CategoriasId = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriasId);
            return View(producto);
        }

        // GET: ProductoController/Edit/5
        [HttpGet("/Producto/Editar/{id:long}")]
        public async Task<IActionResult> Editar(long? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            ViewBag.CategoriasId = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriasId);
            return View(producto);
        }

        // POST: ProductoController/Edit/5
        [HttpPost("/Producto/Editar/{id:long}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(long id, Producto producto)
        {
            if (id != producto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    producto.UsuarioId = HttpContext.Session.GetString("USUARIO_ID") ?? "SYSTEM";
                    producto.UltimaModificacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(ProductosLista));
            }

            ViewBag.CategoriasId = new SelectList(_context.Categorias, "Id", "Nombre", producto.CategoriasId);
            return View(producto);
        }

        // GET: ProductoController/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null) return NotFound();

            return View(producto);
        }

        // POST: ProductoController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ProductosLista));
        }

        private bool ProductoExists(long id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
