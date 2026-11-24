using InventarioService.DTOs;

namespace InventarioService.Interfaces
{
    public interface IBodegaService
    {
        Task<List<BodegaDto>> ConsultarTodos();
        Task<BodegaDto?> ConsultarPorId(int id);
        Task<BodegaDto> Crear(CrearBodegaDto dto);
        Task<bool> Actualizar(int id, ActualizarBodegaDto dto);
        Task<bool> Eliminar(int id);
    }
}