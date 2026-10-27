using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proyecto_Restaurante.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // <-- Habilita la gestión de roles
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// --- SEED DE ROLES Y USUARIO INICIAL ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // Definir los roles del restaurante
        string[] rolesPermitidos = { "Administrador", "Mesero", "Cocina" };

        foreach (var rol in rolesPermitidos)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                await roleManager.CreateAsync(new IdentityRole(rol));
            }
        }

        // Crear un usuario Administrador por defecto
        var emailAdmin = "admin@restaurante.com";
        var adminUser = await userManager.FindByEmailAsync(emailAdmin);
        if (adminUser == null)
        {
            var nuevoAdmin = new IdentityUser
            {
                UserName = emailAdmin,
                Email = emailAdmin,
                EmailConfirmed = true
            };

            // Contraseña segura por defecto para el Admin
            var resultado = await userManager.CreateAsync(nuevoAdmin, "Admin123*");
            if (resultado.Succeeded)
            {
                await userManager.AddToRoleAsync(nuevoAdmin, "Administrador");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar los roles y usuarios.");
    }
}
// ---------------------------------------

app.Run();
