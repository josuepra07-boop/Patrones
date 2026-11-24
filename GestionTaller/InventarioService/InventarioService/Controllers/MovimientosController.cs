using InventarioService.DTOs;
using InventarioService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventarioService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientosController : ControllerBase
    {
        private readonly IInventarioService _service;

        public MovimientosController(
            IInventarioService service)
        {
            _service = service;
        }

        [HttpPost("entrada")]
        public async Task<IActionResult> Entrada(
            EntradaDto dto)
        {
            try
            {
                var resultado =
                    await _service.RegistrarEntrada(dto);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }

        [HttpPost("salida")]
        public async Task<IActionResult> Salida(
            SalidaDto dto)
        {
            try
            {
                var resultado =
                    await _service.RegistrarSalida(dto);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }

        [HttpPost("traslado")]
        public async Task<IActionResult> Traslado(
            TrasladoDto dto)
        {
            try
            {
                var resultado =
                    await _service.RegistrarTraslado(dto);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Consultar(
    [FromQuery] string? tipoMovimiento,
    [FromQuery] int? materialId)
        {
            var resultado =
                await _service.ConsultarMovimientos(
                    tipoMovimiento,
                    materialId);

            return Ok(resultado);
        }

        [HttpPost("devolucion")]
        public async Task<IActionResult> Devolucion(
    DevolucionDto dto)
        {
            try
            {
                var resultado =
                    await _service.RegistrarDevolucion(dto);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }

        [HttpPost("ajuste")]
        public async Task<IActionResult> Ajuste(
    AjusteDto dto)
        {
            try
            {
                var resultado =
                    await _service.RegistrarAjuste(dto);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }
    }
}