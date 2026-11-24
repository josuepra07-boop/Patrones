using System.Diagnostics;
using System.Runtime.CompilerServices;
using InventarioService.Data;
using InventarioService.DTOs;
using InventarioService.Patterns.Flyweight;
using Microsoft.EntityFrameworkCore;

namespace InventarioService.Services
{
    /// <summary>
    /// Prueba del patrón Flyweight utilizando TODOS los movimientos reales almacenados
    /// en DB_INVENTARIO.
    ///
    /// IMPORTANTE:
    /// - Primero se consultan todos los MovimientoDetalles desde SQL Server.
    /// - Después comienza la medición.
    /// - La consulta SQL NO forma parte del Stopwatch.
    /// - Ambos endpoints consultan la misma fuente y no reciben parámetros.
    ///
    /// SIN Flyweight: cada MovimientoDetalle crea su propio objeto Material.
    /// CON Flyweight: los detalles que usan el mismo MaterialId comparten una
    /// única instancia de MaterialFlyweight administrada por la Factory.
    /// </summary>
    public class FlyweightBenchmarkService
    {
        private readonly InventarioDbContext _context;

        public FlyweightBenchmarkService(InventarioDbContext context)
        {
            _context = context;
        }

        // ================================================================
        // SIN FLYWEIGHT
        // ================================================================
        public async Task<FlyweightPruebaResultadoDto> EjecutarSinFlyweight()
        {
            // 1. Todos los datos REALES desde SQL Server. Esta parte NO se mide.
            var movimientos = await ObtenerMovimientosReales();

            CalentarSinFlyweight(movimientos[0]);
            PrepararMemoria();

            // 2. Desde aquí medimos únicamente el procesamiento en memoria.
            var memoriaInicial = GC.GetAllocatedBytesForCurrentThread();
            var reloj = Stopwatch.StartNew();

            var detalles = new List<DetalleSinFlyweight>(movimientos.Count);

            foreach (var registro in movimientos)
            {
                // SIN Flyweight: cada registro crea un Material independiente
                // y nuevas instancias de sus textos descriptivos.
                var materialNuevo = new MaterialSinFlyweight
                {
                    MaterialId = registro.MaterialId,
                    Codigo = CopiarTexto(registro.Codigo),
                    Descripcion = CopiarTexto(registro.Descripcion),
                    UnidadMedida = CopiarTexto(registro.UnidadMedida),
                    Categoria = CopiarTexto(registro.Categoria)
                };

                detalles.Add(new DetalleSinFlyweight
                {
                    Material = materialNuevo,
                    MovimientoId = registro.MovimientoId,
                    MovimientoDetalleId = registro.MovimientoDetalleId,
                    TipoMovimiento = registro.TipoMovimiento,
                    Fecha = registro.Fecha,
                    UsuarioId = registro.UsuarioId,
                    BodegaOrigenId = registro.BodegaOrigenId,
                    BodegaDestinoId = registro.BodegaDestinoId,
                    SolicitudId = registro.SolicitudId,
                    Cantidad = registro.Cantidad
                });
            }

            reloj.Stop();
            var memoriaFinal = GC.GetAllocatedBytesForCurrentThread();
            var memoriaUtilizada = memoriaFinal - memoriaInicial;

            // 3. El JSON se arma DESPUÉS de detener el cronómetro.
            // Se devuelven TODOS los movimientos procesados.
            var movimientosRespuesta = detalles
                .Select(x => new MovimientoRealDto
                {
                    MovimientoId = x.MovimientoId,
                    MovimientoDetalleId = x.MovimientoDetalleId,
                    TipoMovimiento = x.TipoMovimiento,
                    Fecha = x.Fecha,
                    UsuarioId = x.UsuarioId,
                    BodegaOrigenId = x.BodegaOrigenId,
                    BodegaDestinoId = x.BodegaDestinoId,
                    SolicitudId = x.SolicitudId,
                    MaterialId = x.Material.MaterialId,
                    Codigo = x.Material.Codigo,
                    Descripcion = x.Material.Descripcion,
                    UnidadMedida = x.Material.UnidadMedida,
                    Categoria = x.Material.Categoria,
                    Cantidad = x.Cantidad,
                    InstanciaMaterialEnMemoria = RuntimeHelpers.GetHashCode(x.Material)
                })
                .ToList();

            var materialesDiferentes = movimientos
                .Select(x => x.MaterialId)
                .Distinct()
                .Count();

            GC.KeepAlive(detalles);
            GC.KeepAlive(movimientos);

            return new FlyweightPruebaResultadoDto
            {
                TipoPrueba = "SIN FLYWEIGHT",
                RegistrosProcesados = detalles.Count,
                MaterialesDiferentes = materialesDiferentes,
                TiempoProcesamientoMs = Math.Round(reloj.Elapsed.TotalMilliseconds, 3),
                MemoriaAsignadaBytes = memoriaUtilizada,
                MemoriaAsignadaMb = Math.Round(memoriaUtilizada / 1024d / 1024d, 3),
                ObjetosMaterialCreados = detalles.Count,
                Movimientos = movimientosRespuesta,
                Interpretacion =
                    $"Se consultaron {detalles.Count:N0} detalles reales desde SQL Server. " +
                    $"Aunque existen {materialesDiferentes:N0} materiales diferentes, SIN Flyweight " +
                    $"se crearon {detalles.Count:N0} objetos Material independientes: uno por cada detalle."
            };
        }

        // ================================================================
        // CON FLYWEIGHT
        // ================================================================
        public async Task<FlyweightPruebaResultadoDto> EjecutarConFlyweight()
        {
            // 1. Exactamente la misma fuente: todos los MovimientoDetalles reales.
            var movimientos = await ObtenerMovimientosReales();

            CalentarConFlyweight(movimientos[0]);
            PrepararMemoria();

            var factory = new MaterialFlyweightFactory();

            // 2. La medición comienza después de terminar la consulta SQL.
            var memoriaInicial = GC.GetAllocatedBytesForCurrentThread();
            var reloj = Stopwatch.StartNew();

            var detalles = new List<DetalleConFlyweight>(movimientos.Count);

            foreach (var registro in movimientos)
            {
                // La Factory crea el MaterialFlyweight únicamente la primera vez.
                // Si vuelve a aparecer el mismo MaterialId, retorna la misma instancia.
                var materialCompartido = factory.ObtenerOCrear(
                    registro.MaterialId,
                    registro.Codigo,
                    registro.Descripcion,
                    registro.UnidadMedida,
                    registro.Categoria);

                detalles.Add(new DetalleConFlyweight
                {
                    Material = materialCompartido,
                    MovimientoId = registro.MovimientoId,
                    MovimientoDetalleId = registro.MovimientoDetalleId,
                    TipoMovimiento = registro.TipoMovimiento,
                    Fecha = registro.Fecha,
                    UsuarioId = registro.UsuarioId,
                    BodegaOrigenId = registro.BodegaOrigenId,
                    BodegaDestinoId = registro.BodegaDestinoId,
                    SolicitudId = registro.SolicitudId,
                    Cantidad = registro.Cantidad
                });
            }

            reloj.Stop();
            var memoriaFinal = GC.GetAllocatedBytesForCurrentThread();
            var memoriaUtilizada = memoriaFinal - memoriaInicial;

            // 3. Se devuelven TODOS los movimientos después de finalizar la medición.
            var movimientosRespuesta = detalles
                .Select(x => new MovimientoRealDto
                {
                    MovimientoId = x.MovimientoId,
                    MovimientoDetalleId = x.MovimientoDetalleId,
                    TipoMovimiento = x.TipoMovimiento,
                    Fecha = x.Fecha,
                    UsuarioId = x.UsuarioId,
                    BodegaOrigenId = x.BodegaOrigenId,
                    BodegaDestinoId = x.BodegaDestinoId,
                    SolicitudId = x.SolicitudId,
                    MaterialId = x.Material.MaterialId,
                    Codigo = x.Material.Codigo,
                    Descripcion = x.Material.Descripcion,
                    UnidadMedida = x.Material.UnidadMedida,
                    Categoria = x.Material.Categoria,
                    Cantidad = x.Cantidad,
                    InstanciaMaterialEnMemoria = RuntimeHelpers.GetHashCode(x.Material)
                })
                .ToList();

            var materialesDiferentes = movimientos
                .Select(x => x.MaterialId)
                .Distinct()
                .Count();

            var objetosMaterial = factory.CantidadObjetos;

            GC.KeepAlive(detalles);
            GC.KeepAlive(factory);
            GC.KeepAlive(movimientos);

            return new FlyweightPruebaResultadoDto
            {
                TipoPrueba = "CON FLYWEIGHT",
                RegistrosProcesados = detalles.Count,
                MaterialesDiferentes = materialesDiferentes,
                TiempoProcesamientoMs = Math.Round(reloj.Elapsed.TotalMilliseconds, 3),
                MemoriaAsignadaBytes = memoriaUtilizada,
                MemoriaAsignadaMb = Math.Round(memoriaUtilizada / 1024d / 1024d, 3),
                ObjetosMaterialCreados = objetosMaterial,
                Movimientos = movimientosRespuesta,
                Interpretacion =
                    $"Se consultaron {detalles.Count:N0} detalles reales desde SQL Server. " +
                    $"Existen {materialesDiferentes:N0} materiales diferentes y Flyweight creó " +
                    $"solamente {objetosMaterial:N0} objetos Material compartidos. Los registros que " +
                    "usan el mismo MaterialId comparten la misma InstanciaMaterialEnMemoria."
            };
        }

        // ================================================================
        // CONSULTA REAL A SQL SERVER
        // ================================================================
        private async Task<List<MovimientoSqlSource>> ObtenerMovimientosReales()
        {
            var movimientos = await _context.MovimientoDetalles
                .AsNoTracking()
                .Where(x => x.Movimiento != null && x.Material != null)
                .OrderBy(x => x.MovimientoId)
                .ThenBy(x => x.Id)
                .Select(x => new MovimientoSqlSource
                {
                    MovimientoId = x.MovimientoId,
                    MovimientoDetalleId = x.Id,
                    TipoMovimiento = x.Movimiento!.TipoMovimiento,
                    Fecha = x.Movimiento.Fecha,
                    UsuarioId = x.Movimiento.UsuarioId,
                    BodegaOrigenId = x.Movimiento.BodegaOrigenId,
                    BodegaDestinoId = x.Movimiento.BodegaDestinoId,
                    SolicitudId = x.Movimiento.SolicitudId,
                    MaterialId = x.MaterialId,
                    Codigo = x.Material!.Codigo,
                    Descripcion = x.Material.Descripcion,
                    UnidadMedida = x.Material.UnidadMedida,
                    Categoria = x.Material.Categoria != null
                        ? x.Material.Categoria.Nombre
                        : string.Empty,
                    Cantidad = x.Cantidad
                })
                .ToListAsync();

            if (movimientos.Count == 0)
            {
                throw new InvalidOperationException(
                    "No se encontraron MovimientoDetalles en SQL Server para ejecutar la prueba Flyweight.");
            }

            return movimientos;
        }

        // ================================================================
        // CALENTAMIENTO JIT
        // ================================================================
        private static void CalentarSinFlyweight(MovimientoSqlSource registro)
        {
            for (var i = 0; i < 100; i++)
            {
                var material = new MaterialSinFlyweight
                {
                    MaterialId = registro.MaterialId,
                    Codigo = CopiarTexto(registro.Codigo),
                    Descripcion = CopiarTexto(registro.Descripcion),
                    UnidadMedida = CopiarTexto(registro.UnidadMedida),
                    Categoria = CopiarTexto(registro.Categoria)
                };

                _ = new DetalleSinFlyweight
                {
                    Material = material,
                    MovimientoId = registro.MovimientoId,
                    MovimientoDetalleId = registro.MovimientoDetalleId,
                    TipoMovimiento = registro.TipoMovimiento,
                    Fecha = registro.Fecha,
                    UsuarioId = registro.UsuarioId,
                    BodegaOrigenId = registro.BodegaOrigenId,
                    BodegaDestinoId = registro.BodegaDestinoId,
                    SolicitudId = registro.SolicitudId,
                    Cantidad = registro.Cantidad
                };
            }
        }

        private static void CalentarConFlyweight(MovimientoSqlSource registro)
        {
            var factory = new MaterialFlyweightFactory();

            for (var i = 0; i < 100; i++)
            {
                var material = factory.ObtenerOCrear(
                    registro.MaterialId,
                    registro.Codigo,
                    registro.Descripcion,
                    registro.UnidadMedida,
                    registro.Categoria);

                _ = new DetalleConFlyweight
                {
                    Material = material,
                    MovimientoId = registro.MovimientoId,
                    MovimientoDetalleId = registro.MovimientoDetalleId,
                    TipoMovimiento = registro.TipoMovimiento,
                    Fecha = registro.Fecha,
                    UsuarioId = registro.UsuarioId,
                    BodegaOrigenId = registro.BodegaOrigenId,
                    BodegaDestinoId = registro.BodegaDestinoId,
                    SolicitudId = registro.SolicitudId,
                    Cantidad = registro.Cantidad
                };
            }
        }

        // ================================================================
        // UTILIDADES
        // ================================================================
        private static string CopiarTexto(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return string.Empty;

            // Fuerza una nueva instancia de string para representar realmente
            // información repetida en el escenario SIN Flyweight.
            return string.Create(
                texto.Length,
                texto,
                static (destino, origen) => origen.AsSpan().CopyTo(destino));
        }

        private static void PrepararMemoria()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private sealed class MovimientoSqlSource
        {
            public int MovimientoId { get; set; }
            public int MovimientoDetalleId { get; set; }
            public string TipoMovimiento { get; set; } = string.Empty;
            public DateTime Fecha { get; set; }
            public string UsuarioId { get; set; } = string.Empty;
            public int? BodegaOrigenId { get; set; }
            public int? BodegaDestinoId { get; set; }
            public int? SolicitudId { get; set; }
            public int MaterialId { get; set; }
            public string Codigo { get; set; } = string.Empty;
            public string Descripcion { get; set; } = string.Empty;
            public string UnidadMedida { get; set; } = string.Empty;
            public string Categoria { get; set; } = string.Empty;
            public decimal Cantidad { get; set; }
        }
    }
}
