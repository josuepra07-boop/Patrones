using InventarioService.DTOs;

namespace InventarioService.Patterns.Observer
{
    public class AuditoriaMaterialObserver : IMaterialCreadoObserver
    {
        private readonly ILogger<AuditoriaMaterialObserver> _logger;

        public AuditoriaMaterialObserver(ILogger<AuditoriaMaterialObserver> logger)
        {
            _logger = logger;
        }

        public string Nombre => nameof(AuditoriaMaterialObserver);

        public Task<ObserverNotificacionDto> ActualizarAsync(MaterialDto material)
        {
            var mensaje = $"Auditoría: material {material.Codigo} creado con Id {material.Id}.";

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
