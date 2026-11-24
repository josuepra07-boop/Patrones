namespace InventarioService.DTOs
{
    public class BodegaDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Ubicacion { get; set; }
        public bool Activo { get; set; }
    }

    public class CrearBodegaDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Ubicacion { get; set; }
    }

    public class ActualizarBodegaDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Ubicacion { get; set; }
        public bool Activo { get; set; }
    }
}