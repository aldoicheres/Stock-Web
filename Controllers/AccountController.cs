using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using StockWeb.Data;

namespace StockWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly StockDbContext _context; // Tu DbContext de SQLite

        public AccountController(StockDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login (Muestra la vista)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login (Valida datos e inicia sesión)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string usuario, string password)
        {
            // 1. Validar usuario en la base de datos (SQLite)
            var user = _context.Usuarios.FirstOrDefault(u => u.USUARIO == usuario);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PASSWORDHASH)) // Nota: Idealmente comparar con hash
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                return View();
            }

            // 2. Crear las identidades/Claims para la Cookie de sesión
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.USUARIO),
                new Claim(ClaimTypes.Role, user.ROL.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // 3. Iniciar sesión y guardar Cookie en el navegador
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity));

            HttpContext.Session.SetString("USUARIO_ID", user.Id.ToString());

            // 4. Redirigir al index
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("USUARIO_ID");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}