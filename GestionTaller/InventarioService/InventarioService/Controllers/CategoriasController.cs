using InventarioService.DTOs;
using InventarioService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventarioService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _service;

        public CategoriasController(ICategoriaService service)
        {
            _service = service;
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
            CrearCategoriaDto dto)
        {
            var resultado =
                await _service.Crear(dto);

            return CreatedAtAction(
                nameof(Consultar),
                new { id = resultado.Id },
                resultado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(
            int id,
            ActualizarCategoriaDto dto)
        {
            var resultado =
                await _service.Actualizar(id, dto);

            if (!resultado)
                return NotFound();

            return NoContent();
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