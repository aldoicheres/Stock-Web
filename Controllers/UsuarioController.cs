using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockWeb.Data;
using StockWeb.Models;
using System.Xml.Linq;
namespace StockWeb.Controllers
{
    [Authorize(Roles = "1")]
    public class UsuarioController : Controller
    {
        private readonly StockDbContext _context;
        // Inyección de dependencias del DbContext
        public UsuarioController(StockDbContext context)
        {
            _context = context;
        }
       
        // GET: /Usuarios/UsuariosLista
        [HttpGet("/Usuario/UsuariosLista")]
        public IActionResult UsuariosLista()
        {
            var usuarios = _context.Usuarios.ToList();
            return View(usuarios);
        }

        // 2. FORMULARIO AGREGAR (GET)
        public IActionResult Crear()
        {
            return View();
        }

        // 2. FORMULARIO AGREGAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
public async Task<IActionResult> Editar(Usuario usuario, string password)
        {
            if (ModelState.IsValid)
            {
          
    
                // Encriptar contraseña antes de guardar
                usuario.PASSWORDHASH = BCrypt.Net.BCrypt.HashPassword(password);
                usuario.USUARIO_ID = HttpContext.Session.GetString("USUARIO_ID") ?? "SYSTEM";
                usuario.ULTIMA_MODIFICACION = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(UsuariosLista));
            }
            return View(usuario);
        }

        // FORMULARIO EDITAR (GET)
        [HttpGet("/Usuario/Editar/{id:long}")]
        public async Task<IActionResult> Editar(long? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        //  FORMULARIO EDITAR (POST)
        [HttpPost("/Usuario/Editar/{id:long}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
            long id,
            [FromForm(Name = "USUARIO")] string usuarioNombre,
            [FromForm(Name = "EMAIL")] string email,
            [FromForm(Name = "ROL")] string rol,
            string? nuevaPassword)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioExistente = await _context.Usuarios.FindAsync(id);
                    if (usuarioExistente == null) return NotFound();

                    // Actualizar campos
                    usuarioExistente.USUARIO = usuarioNombre;
                    usuarioExistente.EMAIL = email;
                    usuarioExistente.ROL = rol;
                    usuarioExistente.ULTIMA_MODIFICACION = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    // Si escribió una contraseña nueva, la encriptamos
                    if (!string.IsNullOrEmpty(nuevaPassword))
                    {
                        usuarioExistente.PASSWORDHASH = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);
                    }

                    _context.Update(usuarioExistente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Usuarios.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return Redirect("/Usuario/UsuariosLista");
            }
            var usuarioActual = await _context.Usuarios.FindAsync(id);
            return View(usuarioActual);
        }
    }


}

