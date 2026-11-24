using InventarioService.DTOs;

namespace InventarioService.Interfaces
{
    public interface IMaterialService
    {
        Task<List<MaterialDto>> ConsultarTodos();
        Task<MaterialDto?> ConsultarPorId(int id);
        Task<MaterialDto> Crear(CrearMaterialDto dto);
        Task<bool> Actualizar(int id, ActualizarMaterialDto dto);
        Task<bool> Eliminar(int id);
    }
}