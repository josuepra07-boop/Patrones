namespace InventarioService.Patterns.Flyweight
{
    /// <summary>
    /// Flyweight: contiene el estado intrínseco del material,
    /// es decir, la información que puede ser compartida por muchos detalles.
    /// </summary>
    public sealed class MaterialFlyweight
    {
        public int MaterialId { get; }
        public string Codigo { get; }
        public string Descripcion { get; }
        public string UnidadMedida { get; }
        public string Categoria { get; }

        public MaterialFlyweight(
            int materialId,
            string codigo,
            string descripcion,
            string unidadMedida,
            string categoria)
        {
            MaterialId = materialId;
            Codigo = codigo;
            Descripcion = descripcion;
            UnidadMedida = unidadMedida;
            Categoria = categoria;
        }
    }
}
