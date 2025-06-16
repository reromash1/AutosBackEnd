using System.ComponentModel.DataAnnotations;

namespace Autos.DTOs
{
    public class VentaCrearDTO
    {
        [Required(ErrorMessage = "El ID del modelo es obligatorio")]
        public int ModeloCarroId { get; set; }

        [Required(ErrorMessage = "El ID del cliente es obligatorio")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "El precio de venta es obligatorio")]
        [Range(1, 10000000, ErrorMessage = "El precio de venta debe ser positivo")]
        public decimal PrecioVenta { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, 100, ErrorMessage = "La cantidad debe ser entre 1 y 100")]
        public int Cantidad { get; set; } = 1;
    }
}