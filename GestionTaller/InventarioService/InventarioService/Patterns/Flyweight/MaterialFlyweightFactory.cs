namespace InventarioService.Patterns.Flyweight
{
    /// <summary>
    /// Factory del patrón Flyweight. Mantiene una única instancia
    /// de MaterialFlyweight por MaterialId durante el procesamiento.
    /// </summary>
    public sealed class MaterialFlyweightFactory
    {
        private readonly Dictionary<int, MaterialFlyweight> _materiales = new();

        public MaterialFlyweight ObtenerOCrear(
            int materialId,
            string codigo,
            string descripcion,
            string unidadMedida,
            string categoria)
        {
            if (_materiales.TryGetValue(materialId, out var existente))
                return existente;

            var nuevo = new MaterialFlyweight(
                materialId,
                codigo,
                descripcion,
                unidadMedida,
                categoria);

            _materiales.Add(materialId, nuevo);

            return nuevo;
        }

        public int CantidadObjetos => _materiales.Count;
    }
}
