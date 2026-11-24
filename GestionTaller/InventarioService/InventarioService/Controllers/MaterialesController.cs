using InventarioService.DTOs;
using InventarioService.Interfaces;
using InventarioService.Patterns.Command;
using InventarioService.Patterns.Observer;
using Microsoft.AspNetCore.Mvc;

namespace InventarioService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialesController : ControllerBase
    {
        private readonly IMaterialService _service;
        private readonly CommandInvoker _commandInvoker;
        private readonly MaterialCreadoSubject _materialCreadoSubject;

        public MaterialesController(
            IMaterialService service,
            CommandInvoker commandInvoker,
            MaterialCreadoSubject materialCreadoSubject)
        {
            _service = service;
            _commandInvoker = commandInvoker;
            _materialCreadoSubject = materialCreadoSubject;
        }

        [HttpGet]
        public async Task<IActionResult> Consultar()
        {
            return Ok(await _service.ConsultarTodos());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Consultar(int id)
        {
            var resultado =
                await _service.ConsultarPorId(id);

            if (resultado == null)
                return NotFound();

            return Ok(resultado);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(
            CrearMaterialDto dto)
        {
            try
            {
                // CLIENT: crea/configura el comando.
                var command = new CrearMaterialCommand(
                    _service,
                    _materialCreadoSubject,
                    dto);

                // INVOKER: ejecuta el comando sin conocer la lógica interna.
                var resultado =
                    await _commandInvoker.EjecutarAsync(command);

                return CreatedAtAction(
                    nameof(Consultar),
                    new { id = resultado.Id },
                    resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(
            int id,
            ActualizarMaterialDto dto)
        {
            try
            {
                var resultado =
                    await _service.Actualizar(id, dto);

                if (!resultado)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    mensaje = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado =
                await _service.Eliminar(id);

            if (!resultado)
                return NotFound();

            return NoContent();
        }
    }
}
