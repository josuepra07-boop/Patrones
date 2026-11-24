using InventarioService.DTOs;

namespace InventarioService.Patterns.Observer
{
    public interface IMaterialCreadoObserver
    {
        string Nombre { get; }
        Task<ObserverNotificacionDto> ActualizarAsync(MaterialDto material);
    }
}
