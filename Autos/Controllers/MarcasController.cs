using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Autos.Data;
using Autos.Models;
using Autos.DTOs;

namespace Autos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarcasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MarcasController(AppDbContext context) => _context = context;

        // GET: api/marcas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarcaDTO>>> Get()
        {
            return await _context.Marca
                .Select(m => new MarcaDTO
                {
                    MarcaId = m.MarcaId,
                    Nombre = m.Nombre,
                    Descripcion = m.Descripcion
                })
                .ToListAsync();
        }

        // GET: api/marcas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MarcaDTO>> Get(int id)
        {
            var marca = await _context.Marca.FindAsync(id);
            if (marca == null) return NotFound("Marca no encontrada");

            return new MarcaDTO
            {
                MarcaId = marca.MarcaId,
                Nombre = marca.Nombre,
                Descripcion = marca.Descripcion
            };
        }

        // POST: api/marcas
        [HttpPost]
        public async Task<ActionResult<MarcaDTO>> Post(MarcaDTO dto)
        {
            var marca = new Marca
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion
            };

            _context.Marca.Add(marca);
            await _context.SaveChangesAsync();

            dto.MarcaId = marca.MarcaId;
            return CreatedAtAction(nameof(Get), new { id = marca.MarcaId }, dto);
        }

        // PUT: api/marcas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, MarcaDTO dto)
        {
            if (id != dto.MarcaId) return BadRequest("ID no coincide");

            var marca = await _context.Marca.FindAsync(id);
            if (marca == null) return NotFound("Marca no encontrada");

            marca.Nombre = dto.Nombre;
            marca.Descripcion = dto.Descripcion;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MarcaExiste(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/marcas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var marca = await _context.Marca.FindAsync(id);
            if (marca == null) return NotFound("Marca no encontrada");

            if (await _context.ModeloCarro.AnyAsync(m => m.MarcaId == id))
                return BadRequest("No se puede eliminar: Existen modelos asociados");

            _context.Marca.Remove(marca);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MarcaExiste(int id) => _context.Marca.Any(e => e.MarcaId == id);
    }
}