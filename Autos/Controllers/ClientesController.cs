using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Autos.Data;
using Autos.Models;
using Autos.DTOs;

namespace Autos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientesController(AppDbContext context) => _context = context;

        // GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> GetClientes()
        {
            return await _context.Cliente
                .Select(c => new ClienteDTO
                {
                    ClienteId = c.ClienteId,
                    Nombre = c.Nombre,
                    Email = c.Email,
                    Telefono = c.Telefono,
                    VentasRealizadas = _context.Venta
                        .Count(v => v.ClienteId == c.ClienteId)
                })
                .ToListAsync();
        }

        // GET: api/clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDTO>> GetCliente(int id)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null) return NotFound("Cliente no encontrado");

            return new ClienteDTO
            {
                ClienteId = cliente.ClienteId,
                Nombre = cliente.Nombre,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                VentasRealizadas = await _context.Venta
                    .CountAsync(v => v.ClienteId == id)
            };
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<ActionResult<ClienteDTO>> PostCliente(ClienteCrearDTO dto)
        {
            var cliente = new Cliente
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                Telefono = dto.Telefono
            };

            _context.Cliente.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCliente), new { id = cliente.ClienteId },
                new ClienteDTO
                {
                    ClienteId = cliente.ClienteId,
                    Nombre = cliente.Nombre,
                    Email = cliente.Email,
                    Telefono = cliente.Telefono,
                    VentasRealizadas = 0
                });
        }

        // PUT: api/clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, ClienteDTO dto)
        {
            if (id != dto.ClienteId) return BadRequest("ID no coincide");

            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null) return NotFound("Cliente no encontrado");

            cliente.Nombre = dto.Nombre;
            cliente.Email = dto.Email;
            cliente.Telefono = dto.Telefono;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Cliente.FindAsync(id);
            if (cliente == null) return NotFound("Cliente no encontrado");

            // Verificar ventas asociadas
            if (await _context.Venta.AnyAsync(v => v.ClienteId == id))
                return BadRequest("No se puede eliminar: Tiene ventas registradas");

            _context.Cliente.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClienteExists(int id) => _context.Cliente.Any(e => e.ClienteId == id);
    }
}