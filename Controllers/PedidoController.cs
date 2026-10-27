
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto_Restaurante.Data;
using Proyecto_Restaurante.Models;

[Authorize]
public class PedidoController : Controller
{
    private readonly ApplicationDbContext _context;

    public PedidoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: PEDIDOS
    public async Task<IActionResult> Index()    
    {
        var applicationDbContext = _context.Pedidos.Include(p => p.Mesa);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: PEDIDOS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var pedido = await _context.Pedidos
            .Include(p => p.Mesa)
            .Include(p => p.Detalles)
            .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (pedido == null)
        {
            return NotFound();
        }

        return View(pedido);
    }

    // GET: PEDIDOS/Create
    public IActionResult Create()
    {
        var mesas = _context.Mesas
        .Select(m => new {
            Id = m.Id,
            TextoMostrar = $"Mesa {m.Numero} ({m.Estado})"
        })
        .ToList();

        ViewData["MesaId"] = new SelectList(mesas, "Id", "TextoMostrar");

        // Cargar los productos disponibles para la comanda
        ViewBag.Productos = _context.Productos.Where(p => p.Disponible).ToList();

        return View();
    }

    // POST: PEDIDOS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Pedido pedido)
    {
        if (ModelState.IsValid)
        {
            pedido.FechaHora = DateTime.Now;

            // Recalcular el total exacto desde los detalles enviados
            if (pedido.Detalles != null && pedido.Detalles.Any())
            {
                pedido.Total = pedido.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
            }

            _context.Add(pedido);

            // Cambiar estado de la mesa a Ocupada
            var mesa = await _context.Mesas.FindAsync(pedido.MesaId);
            if (mesa != null)
            {
                mesa.Estado = "Ocupada";
                _context.Update(mesa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["MesaId"] = new SelectList(_context.Mesas, "Id", "Numero", pedido.MesaId);
        ViewBag.Productos = _context.Productos.Where(p => p.Disponible).ToList();
        return View(pedido);
    }

    // GET: PEDIDOS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // Incluimos los detalles y los productos vinculados
        var pedido = await _context.Pedidos
            .Include(p => p.Detalles)
            .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (pedido == null)
        {
            return NotFound();
        }

        var mesas = _context.Mesas
            .Select(m => new {
                Id = m.Id,
                TextoMostrar = $"Mesa {m.Numero} ({m.Estado})"
            })
            .ToList();

        ViewData["MesaId"] = new SelectList(mesas, "Id", "TextoMostrar", pedido.MesaId);
        ViewBag.Productos = _context.Productos.Where(p => p.Disponible).ToList();

        return View(pedido);
    }

    // POST: PEDIDOS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Pedido pedido)
    {
        if (id != pedido.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Eliminar los detalles anteriores para reemplazarlos con la comanda actualizada
                var detallesAnteriores = _context.DetallesPedidos.Where(d => d.PedidoId == id);
                _context.DetallesPedidos.RemoveRange(detallesAnteriores);

                // Recalcular el total
                if (pedido.Detalles != null && pedido.Detalles.Any())
                {
                    pedido.Total = pedido.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
                    foreach (var detalle in pedido.Detalles)
                    {
                        detalle.PedidoId = id;
                        _context.DetallesPedidos.Add(detalle);
                    }
                }
                else
                {
                    pedido.Total = 0;
                }

                _context.Update(pedido);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Pedidos.Any(e => e.Id == pedido.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        ViewData["MesaId"] = new SelectList(_context.Mesas, "Id", "Numero", pedido.MesaId);
        ViewBag.Productos = _context.Productos.Where(p => p.Disponible).ToList();
        return View(pedido);
    }

    [Authorize(Roles = "Administrador")]
    // GET: PEDIDOS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var pedido = await _context.Pedidos
            .FirstOrDefaultAsync(m => m.Id == id);
        if (pedido == null)
        {
            return NotFound();
        }

        return View(pedido);
    }

    [Authorize(Roles = "Administrador")]
    // POST: PEDIDOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido != null)
        {
            _context.Pedidos.Remove(pedido);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PedidoExists(int? id)
    {
        return _context.Pedidos.Any(e => e.Id == id);
    }

    // GET: PEDIDOS/Cocina
    [Authorize(Roles = "Administrador, Cocina, Mesero")]
    public async Task<IActionResult> Cocina()
    {
        var pedidosPendientes = await _context.Pedidos
            .Include(p => p.Mesa)
            .Include(p => p.Detalles)
            .ThenInclude(d => d.Producto)
            .Where(p => p.Estado == "Pendiente" || p.Estado == "En Preparación")
            .OrderBy(p => p.FechaHora)
            .ToListAsync();

        return View(pedidosPendientes);
    }

    // POST: PEDIDOS/CambiarEstado
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrador, Cocina, Mesero")]
    public async Task<IActionResult> CambiarEstado(int id, string nuevoEstado)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido == null)
        {
            return NotFound();
        }

        pedido.Estado = nuevoEstado;
        _context.Update(pedido);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Cocina));
    }
}
