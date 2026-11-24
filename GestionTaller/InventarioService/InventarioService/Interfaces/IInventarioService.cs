using InventarioService.DTOs;

namespace InventarioService.Interfaces
{
    public interface IInventarioService
    {
        Task<List<InventarioDto>> Consultar(
            int? bodegaId,
            int? materialId);

        Task<List<MovimientoConsultaDto>> ConsultarMovimientos(
            string? tipoMovimiento,
            int? materialId);

        Task<MovimientoResultadoDto> RegistrarEntrada(
            EntradaDto dto);

        Task<MovimientoResultadoDto> RegistrarSalida(
            SalidaDto dto);

        Task<MovimientoResultadoDto> RegistrarTraslado(
            TrasladoDto dto);

        Task<MovimientoResultadoDto> RegistrarDevolucion(
            DevolucionDto dto);

        Task<MovimientoResultadoDto> RegistrarAjuste(
            AjusteDto dto);
    }
}