using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Autos.Data;
using Autos.Models;
using Autos.DTOs;

namespace Autos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelosCarroController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ModelosCarroController(AppDbContext context) => _context = context;

        // GET: api/modeloscarro
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModeloCarroDTO>>> GetModelos()
        {
            return await _context.ModeloCarro
                .Select(m => new ModeloCarroDTO
                {
                    ModeloCarroId = m.ModeloCarroId,
                    Nombre = m.Nombre,
                    Año = m.Año,
                    Color = m.Color,
                    Precio = m.Precio,
                    Stock = m.Stock,
                    MarcaId = m.MarcaId,
                    // Obtener nombre de marca manualmente
                    MarcaNombre = _context.Marca
                        .Where(b => b.MarcaId == m.MarcaId)
                        .Select(b => b.Nombre)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        // GET: api/modeloscarro/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ModeloCarroDTO>> GetModelo(int id)
        {
            var modelo = await _context.ModeloCarro.FindAsync(id);
            if (modelo == null) return NotFound("Modelo no encontrado");

            // Obtener marca manualmente
            var marca = await _context.Marca
                .FirstOrDefaultAsync(b => b.MarcaId == modelo.MarcaId);

            return new ModeloCarroDTO
            {
                ModeloCarroId = modelo.ModeloCarroId,
                Nombre = modelo.Nombre,
                Año = modelo.Año,
                Color = modelo.Color,
                Precio = modelo.Precio,
                Stock = modelo.Stock,
                MarcaId = modelo.MarcaId,
                MarcaNombre = marca?.Nombre
            };
        }

        // POST: api/modeloscarro
        [HttpPost]
        public async Task<ActionResult<ModeloCarroDTO>> PostModelo(ModeloCarroCrearDTO dto)
        {
            // Validar marca usando solo ID
            bool marcaExiste = await _context.Marca
                .AnyAsync(b => b.MarcaId == dto.MarcaId);

            if (!marcaExiste)
                return BadRequest("Marca no válida");

            var modelo = new ModeloCarro
            {
                Nombre = dto.Nombre,
                Año = dto.Año,
                Color = dto.Color,
                Precio = dto.Precio,
                Stock = dto.Stock,
                MarcaId = dto.MarcaId // Solo ID, sin referencia a objeto
            };

            _context.ModeloCarro.Add(modelo);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetModelo), new { id = modelo.ModeloCarroId },
                new ModeloCarroDTO
                {
                    ModeloCarroId = modelo.ModeloCarroId,
                    Nombre = modelo.Nombre,
                    Año = modelo.Año,
                    Color = modelo.Color,
                    Precio = modelo.Precio,
                    Stock = modelo.Stock,
                    MarcaId = modelo.MarcaId
                    // MarcaNombre se puede omitir o cargar en frontend
                });
        }

        // PUT: api/modeloscarro/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModelo(int id, ModeloCarroDTO dto)
        {
            if (id != dto.ModeloCarroId)
                return BadRequest("ID no coincide");

            // Validar marca por ID
            bool marcaValida = await _context.Marca
                .AnyAsync(b => b.MarcaId == dto.MarcaId);

            if (!marcaValida)
                return BadRequest("Marca no válida");

            var modelo = new ModeloCarro
            {
                ModeloCarroId = dto.ModeloCarroId,
                Nombre = dto.Nombre,
                Año = dto.Año,
                Color = dto.Color,
                Precio = dto.Precio,
                Stock = dto.Stock,
                MarcaId = dto.MarcaId // Solo ID
            };

            _context.Entry(modelo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModeloExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/modeloscarro/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModelo(int id)
        {
            var modelo = await _context.ModeloCarro.FindAsync(id);
            if (modelo == null) return NotFound("Modelo no encontrado");

            // Verificar ventas usando solo ID
            bool tieneVentas = await _context.Venta
                .AnyAsync(v => v.ModeloCarroId == id);

            if (tieneVentas)
                return BadRequest("No se puede eliminar: Existen ventas asociadas");

            _context.ModeloCarro.Remove(modelo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ModeloExists(int id) =>
            _context.ModeloCarro.Any(e => e.ModeloCarroId == id);
    }
}