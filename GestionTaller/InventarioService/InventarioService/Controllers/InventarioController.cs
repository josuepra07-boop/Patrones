using InventarioService.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventarioService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventarioController : ControllerBase
    {
        private readonly IInventarioService _service;

        public InventarioController(
            IInventarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Consultar(
            [FromQuery] int? bodegaId,
            [FromQuery] int? materialId)
        {
            var resultado =
                await _service.Consultar(
                    bodegaId,
                    materialId);

            return Ok(resultado);
        }
    }
}