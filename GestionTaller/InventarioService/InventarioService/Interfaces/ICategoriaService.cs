using InventarioService.DTOs;

namespace InventarioService.Interfaces
{
    public interface ICategoriaService
    {
        Task<List<CategoriaDto>> ConsultarTodos();
        Task<CategoriaDto?> ConsultarPorId(int id);
        Task<CategoriaDto> Crear(CrearCategoriaDto dto);
        Task<bool> Actualizar(int id, ActualizarCategoriaDto dto);
        Task<bool> Eliminar(int id);
    }
}