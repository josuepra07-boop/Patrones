namespace InventarioService.Models
{
    public class Bodega
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string? Ubicacion { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<Inventario> Inventarios { get; set; } = new List<Inventario>();
    }
}