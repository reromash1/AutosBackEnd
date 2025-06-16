using System.ComponentModel.DataAnnotations;

namespace Autos.DTOs
{
    public class ClienteDTO
    {
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; }

        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Formato de teléfono inválido")]
        public string Telefono { get; set; }

        public int VentasRealizadas { get; set; } // Contador de ventas
    }
}