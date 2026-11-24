using InventarioService.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventarioService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatronesController : ControllerBase
    {
        private readonly FlyweightBenchmarkService _flyweightService;

        public PatronesController(FlyweightBenchmarkService flyweightService)
        {
            _flyweightService = flyweightService;
        }

        /// <summary>
        /// Consulta TODOS los movimientos reales desde SQL Server y los procesa SIN Flyweight.
        /// No recibe parámetros ni body.
        /// </summary>
        [HttpGet("flyweight/sin")]
        public async Task<IActionResult> EjecutarSinFlyweight()
        {
            try
            {
                var resultado = await _flyweightService.EjecutarSinFlyweight();
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

        /// <summary>
        /// Consulta TODOS los movimientos reales desde SQL Server y los procesa CON Flyweight.
        /// No recibe parámetros ni body.
        /// </summary>
        [HttpGet("flyweight/con")]
        public async Task<IActionResult> EjecutarConFlyweight()
        {
            try
            {
                var resultado = await _flyweightService.EjecutarConFlyweight();
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
