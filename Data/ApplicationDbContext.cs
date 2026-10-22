using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Proyecto_Restaurante.Models;

namespace Proyecto_Restaurante.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    // Registrar la tabla de Productos
    public DbSet<Producto> Productos { get; set; }
}
