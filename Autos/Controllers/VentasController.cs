using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Autos.Data;
using Autos.Models;
using Autos.DTOs;

namespace Autos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context) => _context = context;

        // GET: api/ventas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VentaDTO>>> GetVentas()
        {
            return await _context.Venta
                .Select(v => new VentaDTO
                {
                    VentaId = v.VentaId,
                    FechaVenta = v.FechaVenta,
                    PrecioVenta = v.PrecioVenta,
                    Cantidad = v.Cantidad,
                    ModeloCarroId = v.ModeloCarroId,
                    ClienteId = v.ClienteId,
                    // Obtener datos relacionados manualmente
                    ModeloNombre = _context.ModeloCarro
                        .Where(m => m.ModeloCarroId == v.ModeloCarroId)
                        .Select(m => m.Nombre)
                        .FirstOrDefault(),
                    ModeloColor = _context.ModeloCarro
                        .Where(m => m.ModeloCarroId == v.ModeloCarroId)
                        .Select(m => m.Color)
                        .FirstOrDefault(),
                    MarcaId = _context.ModeloCarro
                        .Where(m => m.ModeloCarroId == v.ModeloCarroId)
                        .Select(m => m.MarcaId)
                        .FirstOrDefault(),
                    MarcaNombre = _context.Marca
                        .Where(b => b.MarcaId == _context.ModeloCarro
                            .Where(m => m.ModeloCarroId == v.ModeloCarroId)
                            .Select(m => m.MarcaId)
                            .FirstOrDefault())
                        .Select(b => b.Nombre)
                        .FirstOrDefault(),
                    ClienteNombre = _context.Cliente
                        .Where(c => c.ClienteId == v.ClienteId)
                        .Select(c => c.Nombre)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        // POST: api/ventas
        [HttpPost]
        public async Task<ActionResult<VentaDTO>> PostVenta(VentaCrearDTO dto)
        {
            // Validar modelo por ID
            var modelo = await _context.ModeloCarro
                .FirstOrDefaultAsync(m => m.ModeloCarroId == dto.ModeloCarroId);

            if (modelo == null)
                return NotFound("Modelo no encontrado");

            if (modelo.Stock < dto.Cantidad)
                return BadRequest($"Stock insuficiente. Disponible: {modelo.Stock}");

            // Validar cliente por ID
            bool clienteExiste = await _context.Cliente
                .AnyAsync(c => c.ClienteId == dto.ClienteId);

            if (!clienteExiste)
                return NotFound("Cliente no encontrado");

            // Crear venta usando solo IDs
            var venta = new Venta
            {
                ModeloCarroId = dto.ModeloCarroId,
                ClienteId = dto.ClienteId,
                PrecioVenta = dto.PrecioVenta,
                Cantidad = dto.Cantidad,
                FechaVenta = DateTime.UtcNow
            };

            // Actualizar stock
            modelo.Stock -= dto.Cantidad;

            _context.Venta.Add(venta);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVentas), new { id = venta.VentaId },
                new VentaDTO
                {
                    VentaId = venta.VentaId,
                    FechaVenta = venta.FechaVenta,
                    PrecioVenta = venta.PrecioVenta,
                    Cantidad = venta.Cantidad,
                    ModeloCarroId = venta.ModeloCarroId,
                    ClienteId = venta.ClienteId
                });
        }
        // GET: api/ventas/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<VentaDTO>> GetVenta(int id)
        {
            var venta = await _context.Venta.FindAsync(id);
            if (venta == null) return NotFound();

            var modelo = await _context.ModeloCarro.FindAsync(venta.ModeloCarroId);
            var marca = await _context.Marca.FindAsync(modelo?.MarcaId);
            var cliente = await _context.Cliente.FindAsync(venta.ClienteId);

            return new VentaDTO
            {
                VentaId = venta.VentaId,
                FechaVenta = venta.FechaVenta,
                PrecioVenta = venta.PrecioVenta,
                Cantidad = venta.Cantidad,
                ModeloCarroId = venta.ModeloCarroId,
                ModeloNombre = modelo?.Nombre,
                ModeloColor = modelo?.Color,
                MarcaId = marca?.MarcaId ?? 0,
                MarcaNombre = marca?.Nombre,
                ClienteId = venta.ClienteId,
                ClienteNombre = cliente?.Nombre
            };
        }

        // PUT: api/ventas/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVenta(int id, VentaCrearDTO dto)
        {
            var venta = await _context.Venta.FindAsync(id);
            if (venta == null) return NotFound("Venta no encontrada");

            var modelo = await _context.ModeloCarro.FindAsync(dto.ModeloCarroId);
            if (modelo == null) return NotFound("Modelo no encontrado");

            var clienteExiste = await _context.Cliente.AnyAsync(c => c.ClienteId == dto.ClienteId);
            if (!clienteExiste) return NotFound("Cliente no encontrado");

            venta.ModeloCarroId = dto.ModeloCarroId;
            venta.ClienteId = dto.ClienteId;
            venta.PrecioVenta = dto.PrecioVenta;
            venta.Cantidad = dto.Cantidad;

            _context.Entry(venta).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ventas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenta(int id)
        {
            var venta = await _context.Venta.FindAsync(id);
            if (venta == null) return NotFound();

            var modelo = await _context.ModeloCarro.FindAsync(venta.ModeloCarroId);
            if (modelo != null)
            {
                modelo.Stock += venta.Cantidad; // Reintegrar stock si se elimina la venta
            }

            _context.Venta.Remove(venta);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}