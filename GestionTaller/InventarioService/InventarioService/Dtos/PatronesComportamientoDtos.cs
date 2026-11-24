namespace InventarioService.DTOs
{
    public class ObserverNotificacionDto
    {
        public string Observer { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }

    public class CrearMaterialCommandResultadoDto : MaterialDto
    {
        public string PatronCommand { get; set; } = string.Empty;
        public string Invoker { get; set; } = string.Empty;
        public List<ObserverNotificacionDto> ObserversNotificados { get; set; } = new();
    }
}
