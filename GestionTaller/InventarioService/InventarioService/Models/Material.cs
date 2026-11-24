namespace InventarioService.Models
{
    public class Material
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public int CategoriaId { get; set; }

        public string UnidadMedida { get; set; } = string.Empty;

        public decimal StockMinimo { get; set; }

        public bool Activo { get; set; } = true;

        public Categoria? Categoria { get; set; }

        public ICollection<Inventario> Inventarios { get; set; } = new List<Inventario>();
    }
}