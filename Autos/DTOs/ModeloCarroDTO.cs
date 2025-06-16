using System.ComponentModel.DataAnnotations;

namespace Autos.DTOs
{
    public class ModeloCarroDTO
    {
        public int ModeloCarroId { get; set; }

        [Required(ErrorMessage = "El nombre del modelo es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El año es obligatorio")]
        [Range(2020, 2025, ErrorMessage = "El año debe estar entre 2020 y 2025")]
        public int Año { get; set; }

        [Required(ErrorMessage = "El color es obligatorio")]
        public string Color { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(1000, 10000000, ErrorMessage = "El precio debe estar entre 1,000 y 10,000,000")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio")]
        [Range(0, 1000, ErrorMessage = "El stock debe ser entre 0 y 1000")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "La marca es obligatoria")]
        public int MarcaId { get; set; }

        public string MarcaNombre { get; set; } // Para mostrar en UI
    }
}
