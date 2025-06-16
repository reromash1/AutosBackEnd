using System.ComponentModel.DataAnnotations;

namespace Autos.Models
{
    public class Venta
    {
        public int VentaId { get; set; }

        [Display(Name = "Fecha de Venta")]
        public DateTime FechaVenta { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "El precio de venta es obligatorio")]
        [Range(1, 10000000, ErrorMessage = "El precio de venta debe ser positivo")]
        [Display(Name = "Precio de Venta")]
        public decimal PrecioVenta { get; set; }

        [Required(ErrorMessage = "El modelo es obligatorio")]
        [Display(Name = "Modelo")]
        public int ModeloCarroId { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, 100, ErrorMessage = "La cantidad debe ser entre 1 y 100")]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; } = 1;
    }
}