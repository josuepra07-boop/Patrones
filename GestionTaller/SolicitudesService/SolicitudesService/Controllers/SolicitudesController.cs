using Microsoft.AspNetCore.Mvc;
using SolicitudesService.DTOs;
using SolicitudesService.Interfaces;

namespace SolicitudesService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudesController : ControllerBase
    {
        private readonly ISolicitudService _service;

        public SolicitudesController(
            ISolicitudService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Consultar(
            [FromQuery] string? estado)
        {
            return Ok(
                await _service.ConsultarTodos(
                    estado));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Consultar(
            int id)
        {
            var resultado =
                await _service.ConsultarPorId(id);

            if (resultado == null)
                return NotFound();

            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(
            CrearSolicitudDto dto)
        {
            try
            {
                var resultado =
                    await _service.Crear(dto);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        mensaje = ex.Message
                    });
            }
        }

        [HttpPost("{id}/clonar")]
        public async Task<IActionResult> Clonar(
            int id,
            ClonarSolicitudDto dto)
        {
            try
            {
                var resultado =
                    await _service.Clonar(
                        id,
                        dto);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        mensaje = ex.Message
                    });
            }
        }

        [HttpPost("{id}/aprobar")]
        public async Task<IActionResult> Aprobar(
            int id,
            AprobarSolicitudDto dto)
        {
            try
            {
                var resultado =
                    await _service.Aprobar(
                        id,
                        dto);

                if (!resultado)
                    return NotFound();

                return Ok(new
                {
                    mensaje =
                        "Solicitud aprobada correctamente."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        mensaje = ex.Message
                    });
            }
        }

        [HttpPost("{id}/rechazar")]
        public async Task<IActionResult> Rechazar(
            int id,
            RechazarSolicitudDto dto)
        {
            try
            {
                var resultado =
                    await _service.Rechazar(
                        id,
                        dto);

                if (!resultado)
                    return NotFound();

                return Ok(new
                {
                    mensaje =
                        "Solicitud rechazada correctamente."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        mensaje = ex.Message
                    });
            }
        }
    }
}