using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Restaurante.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Restaurante.Controllers
{
    [Authorize] // Exige estar autenticado para ver el Dashboard
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Estadísticas de Mesas
            ViewBag.TotalMesas = await _context.Mesas.CountAsync();
            ViewBag.MesasOcupadas = await _context.Mesas.CountAsync(m => m.Estado == "Ocupada");
            ViewBag.MesasDisponibles = await _context.Mesas.CountAsync(m => m.Estado == "Disponible");

            // 2. Estadísticas de Pedidos del Día
            var hoy = System.DateTime.Today;

            // Filtrar los pedidos del día
            var pedidosHoyList = await _context.Pedidos
                .Where(p => p.FechaHora >= hoy)
                .ToListAsync(); // Trae los datos a memoria de forma asíncrona

            ViewBag.PedidosPendientes = pedidosHoyList.Count(p => p.Estado == "Pendiente" || p.Estado == "En Preparación");
            ViewBag.PedidosPreparados = pedidosHoyList.Count(p => p.Estado == "Preparado");

            // Ahora podemos usar Sum en memoria sin que falle SQLite
            ViewBag.TotalVentasHoy = pedidosHoyList.Sum(p => p.Total);

            // 3. Productos activos en catálogo
            ViewBag.TotalProductos = await _context.Productos.CountAsync(p => p.Disponible);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}