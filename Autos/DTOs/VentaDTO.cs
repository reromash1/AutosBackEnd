namespace Autos.DTOs
{
    public class VentaDTO
    {
        public int VentaId { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Cantidad { get; set; }

        // Información relacionada
        public int ModeloCarroId { get; set; }
        public string ModeloNombre { get; set; }
        public string ModeloColor { get; set; }
        public int MarcaId { get; set; }
        public string MarcaNombre { get; set; }

        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; }
    }
}
