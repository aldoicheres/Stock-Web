using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockWeb.Data;
using StockWeb.Models;

namespace StockWeb.Controllers
{
    [Authorize(Roles = "1")]
    public class CategoriaController : Controller
    {
        private readonly StockDbContext _context;
        // Inyección de dependencias del DbContext
        public CategoriaController(StockDbContext context)
        {
            _context = context;
        }
        // GET: CategoriaController

        [HttpGet("/Categoria/CategoriaLista")]
        public IActionResult CategoriasLista()
        {
            var categorias = _context.Categorias.ToList();
            return View("CategoriaLista", categorias);
        }
       
        // FORMULARIO EDITAR (GET)
        [HttpGet("/Categoria/Editar/{id:long}")]
        public async Task<IActionResult> Editar(long? id)
        {
            if (id == null) return NotFound();

            var Categoria = await _context.Categorias.FindAsync(id);
            if (Categoria == null) return NotFound();

            return View(Categoria);
        }

        //  FORMULARIO EDITAR (POST)
        [HttpPost("/Categoria/Editar/{id:long}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            long id,
            [FromForm(Name = "nombre")] string nombre,
            [FromForm(Name = "descripcion")] string descripcion)
          
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var categoriaExistente = await _context.Categorias.FindAsync(id);
                    if (categoriaExistente == null) return NotFound();
                    // Actualizar campos
                    categoriaExistente.Nombre = nombre;
                    categoriaExistente.Descripcion = descripcion;
                    categoriaExistente.UltimaModificacion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    categoriaExistente.UsuarioId = HttpContext.Session.GetString("USUARIO_ID") ?? "SYSTEM";
                    _context.Update(categoriaExistente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Categorias.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                // return Redirect("/Categoria/CategoriaLista");
                return RedirectToAction(nameof(CategoriasLista));
            }
            var CategoriaActual = await _context.Categorias.FindAsync(id);
            return View(CategoriaActual);
        }



        // GET: CategoriaController/nuevo
        public IActionResult Crear()
        {
            return View();
        }
        // POST: CategoriaController/Nuevo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Crear(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                categoria.UsuarioId = HttpContext.Session.GetString("USUARIO_ID")?? "SYSTEM";
                categoria.UltimaModificacion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

                _context.Categorias.Add(categoria);
                _context.SaveChanges();
                return RedirectToAction(nameof(CategoriasLista));
            }
            return View(categoria);
        }
    }




}
