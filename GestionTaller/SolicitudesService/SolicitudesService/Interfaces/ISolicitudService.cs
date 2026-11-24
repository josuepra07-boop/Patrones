using SolicitudesService.DTOs;

namespace SolicitudesService.Interfaces
{
    public interface ISolicitudService
    {
        Task<List<SolicitudDto>> ConsultarTodos(
            string? estado);

        Task<SolicitudDto?> ConsultarPorId(
            int id);

        Task<SolicitudDto> Crear(
            CrearSolicitudDto dto);

        Task<SolicitudDto> Clonar(
            int id,
            ClonarSolicitudDto dto);

        Task<bool> Aprobar(
            int id,
            AprobarSolicitudDto dto);

        Task<bool> Rechazar(
            int id,
            RechazarSolicitudDto dto);

        Task<bool> MarcarAtendida(
            int solicitudId,
            int movimientoId);
    }
}