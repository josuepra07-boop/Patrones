namespace InventarioService.Patterns.Flyweight
{
    /// <summary>
    /// Representación tradicional usada solo para la prueba SIN Flyweight.
    /// Cada detalle crea una nueva instancia con su propia copia de los datos
    /// descriptivos del material.
    /// </summary>
    public sealed class MaterialSinFlyweight
    {
        public int MaterialId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string UnidadMedida { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
    }
}
