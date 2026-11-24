namespace InventarioService.DTOs
{
    public class MaterialDto
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public int CategoriaId { get; set; }

        public string Categoria { get; set; } = string.Empty;

        public string UnidadMedida { get; set; } = string.Empty;

        public decimal StockMinimo { get; set; }

        public bool Activo { get; set; }
    }

    public class CrearMaterialDto
    {
        public string Codigo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public int CategoriaId { get; set; }

        public string UnidadMedida { get; set; } = string.Empty;

        public decimal StockMinimo { get; set; }
    }

    public class ActualizarMaterialDto
    {
        public string Codigo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public int CategoriaId { get; set; }

        public string UnidadMedida { get; set; } = string.Empty;

        public decimal StockMinimo { get; set; }

        public bool Activo { get; set; }
    }
}