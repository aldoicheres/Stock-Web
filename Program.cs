using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using StockWeb.Data;
using StockWeb.Models;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StockDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("CadenaSqlite")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Ruta de redirección si no está logueado
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

var app = builder.Build();


app.UseRouting();
app.UseStaticFiles(); // archivos de la app 
app.UseSession();


app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"); // Iniciar por defecto en el Login



app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<StockDbContext>(); // O ApplicationDbContext

            // La base de datos puede existir como archivo vacío (o no existir todavía).
            // Crear el esquema antes de consultar Usuarios evita el error "no such table".
            context.Database.EnsureCreated();

        // Verifica si ya existe algún usuario registrado
        if (!context.Usuarios.Any())
        {
            // Encriptación de la contraseña 'adminqwerty' con BCrypt
            string passwordPlana = "adminqwerty";
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(passwordPlana);

            var usuarioAdmin = new Usuario
            {
                USUARIO = "admin",
                EMAIL = "adim@admin",
                PASSWORDHASH = passwordHash,
                ROL = "1", // 1 para Rol Administrador
                USUARIO_ID = "SYSTEM",
                ULTIMA_MODIFICACION = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            context.Usuarios.Add(usuarioAdmin);
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al inicializar la base de datos: {ex.Message}");
    }
}

app.MapRazorPages();


app.Run();

