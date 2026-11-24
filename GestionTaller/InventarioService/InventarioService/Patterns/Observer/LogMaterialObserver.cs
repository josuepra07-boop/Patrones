using InventarioService.DTOs;

namespace InventarioService.Patterns.Observer
{
    public class LogMaterialObserver : IMaterialCreadoObserver
    {
        private readonly ILogger<LogMaterialObserver> _logger;

        public LogMaterialObserver(ILogger<LogMaterialObserver> logger)
        {
            _logger = logger;
        }

        public string Nombre => nameof(LogMaterialObserver);

        public Task<ObserverNotificacionDto> ActualizarAsync(MaterialDto material)
        {
            var mensaje = $"Log: se procesó correctamente la creación de {material.Codigo} - {material.Descripcion}.";

            _logger.LogInformation(
                "[Observer] {Observer}: {Mensaje}",
                Nombre,
                mensaje);

            return Task.FromResult(new ObserverNotificacionDto
            {
                Observer = Nombre,
                Mensaje = mensaje
            });
        }
    }
}
