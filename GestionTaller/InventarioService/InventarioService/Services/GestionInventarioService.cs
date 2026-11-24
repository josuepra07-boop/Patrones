using InventarioService.Builders;
using InventarioService.Data;
using InventarioService.DTOs;
using InventarioService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventarioService.Services
{
    public class GestionInventarioService : IInventarioService
    {
        private readonly InventarioDbContext _context;

        public GestionInventarioService(
            InventarioDbContext context)
        {
            _context = context;
        }

        public async Task<List<InventarioDto>> Consultar(
            int? bodegaId,
            int? materialId)
        {
            var query = _context.Inventarios
                .AsNoTracking()
                .Include(x => x.Material)
                .Include(x => x.Bodega)
                .AsQueryable();

            if (bodegaId.HasValue)
            {
                query = query.Where(
                    x => x.BodegaId == bodegaId.Value);
            }

            if (materialId.HasValue)
            {
                query = query.Where(
                    x => x.MaterialId == materialId.Value);
            }

            return await query
                .Select(x => new InventarioDto
                {
                    Id = x.Id,

                    MaterialId = x.MaterialId,

                    CodigoMaterial =
                        x.Material != null
                            ? x.Material.Codigo
                            : string.Empty,

                    DescripcionMaterial =
                        x.Material != null
                            ? x.Material.Descripcion
                            : string.Empty,

                    BodegaId = x.BodegaId,

                    Bodega =
                        x.Bodega != null
                            ? x.Bodega.Nombre
                            : string.Empty,

                    CantidadDisponible =
                        x.CantidadDisponible,

                    FechaActualizacion =
                        x.FechaActualizacion
                })
                .OrderBy(x => x.Bodega)
                .ThenBy(x => x.CodigoMaterial)
                .ToListAsync();
        }

        public async Task<MovimientoResultadoDto> RegistrarEntrada(
            EntradaDto dto)
        {
            await ValidarBodega(dto.BodegaDestinoId);

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var builder = new MovimientoBuilder()
                    .ConTipo("ENTRADA")
                    .ConUsuario(dto.UsuarioId)
                    .ConBodegaDestino(dto.BodegaDestinoId)
                    .ConObservacion(dto.Observacion);

                foreach (var detalle in dto.Detalles)
                {
                    await ValidarMaterial(detalle.MaterialId);

                    if (detalle.Cantidad <= 0)
                    {
                        throw new InvalidOperationException(
                            "La cantidad debe ser mayor que cero.");
                    }

                    var inventario =
                        await ObtenerOCrearInventario(
                            detalle.MaterialId,
                            dto.BodegaDestinoId);

                    inventario.CantidadDisponible +=
                        detalle.Cantidad;

                    inventario.FechaActualizacion =
                        DateTime.Now;

                    builder.AgregarDetalle(
                        detalle.MaterialId,
                        detalle.Cantidad);
                }

                var movimiento = builder.Build();

                _context.Movimientos.Add(movimiento);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new MovimientoResultadoDto
                {
                    MovimientoId = movimiento.Id,
                    TipoMovimiento = movimiento.TipoMovimiento,
                    Fecha = movimiento.Fecha,
                    Mensaje = "Entrada registrada correctamente."
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<MovimientoResultadoDto> RegistrarSalida(
            SalidaDto dto)
        {
            await ValidarBodega(dto.BodegaOrigenId);

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var builder = new MovimientoBuilder()
                    .ConTipo("SALIDA")
                    .ConUsuario(dto.UsuarioId)
                    .ConBodegaOrigen(dto.BodegaOrigenId)
                    .ConSolicitud(dto.SolicitudId)
                    .ConObservacion(dto.Observacion);

                foreach (var detalle in dto.Detalles)
                {
                    await ValidarMaterial(detalle.MaterialId);

                    if (detalle.Cantidad <= 0)
                    {
                        throw new InvalidOperationException(
                            "La cantidad debe ser mayor que cero.");
                    }

                    var inventario =
                        await _context.Inventarios
                            .FirstOrDefaultAsync(x =>
                                x.MaterialId ==
                                    detalle.MaterialId &&
                                x.BodegaId ==
                                    dto.BodegaOrigenId);

                    if (inventario == null)
                    {
                        throw new InvalidOperationException(
                            $"El material {detalle.MaterialId} " +
                            "no tiene inventario en la bodega.");
                    }

                    if (inventario.CantidadDisponible <
                        detalle.Cantidad)
                    {
                        throw new InvalidOperationException(
                            $"Stock insuficiente para el material " +
                            $"{detalle.MaterialId}. " +
                            $"Disponible: " +
                            $"{inventario.CantidadDisponible}.");
                    }

                    inventario.CantidadDisponible -=
                        detalle.Cantidad;

                    inventario.FechaActualizacion =
                        DateTime.Now;

                    builder.AgregarDetalle(
                        detalle.MaterialId,
                        detalle.Cantidad);
                }

                var movimiento = builder.Build();

                _context.Movimientos.Add(movimiento);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new MovimientoResultadoDto
                {
                    MovimientoId = movimiento.Id,
                    TipoMovimiento = movimiento.TipoMovimiento,
                    Fecha = movimiento.Fecha,
                    Mensaje = "Salida registrada correctamente."
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<MovimientoResultadoDto> RegistrarTraslado(
            TrasladoDto dto)
        {
            if (dto.BodegaOrigenId ==
                dto.BodegaDestinoId)
            {
                throw new InvalidOperationException(
                    "La bodega origen y destino deben ser diferentes.");
            }

            await ValidarBodega(dto.BodegaOrigenId);

            await ValidarBodega(dto.BodegaDestinoId);

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var builder = new MovimientoBuilder()
                    .ConTipo("TRASLADO")
                    .ConUsuario(dto.UsuarioId)
                    .ConBodegaOrigen(dto.BodegaOrigenId)
                    .ConBodegaDestino(dto.BodegaDestinoId)
                    .ConObservacion(dto.Observacion);

                foreach (var detalle in dto.Detalles)
                {
                    await ValidarMaterial(detalle.MaterialId);

                    if (detalle.Cantidad <= 0)
                    {
                        throw new InvalidOperationException(
                            "La cantidad debe ser mayor que cero.");
                    }

                    var inventarioOrigen =
                        await _context.Inventarios
                            .FirstOrDefaultAsync(x =>
                                x.MaterialId ==
                                    detalle.MaterialId &&
                                x.BodegaId ==
                                    dto.BodegaOrigenId);

                    if (inventarioOrigen == null)
                    {
                        throw new InvalidOperationException(
                            $"El material {detalle.MaterialId} " +
                            "no existe en la bodega origen.");
                    }

                    if (inventarioOrigen.CantidadDisponible <
                        detalle.Cantidad)
                    {
                        throw new InvalidOperationException(
                            $"Stock insuficiente para el material " +
                            $"{detalle.MaterialId}.");
                    }

                    inventarioOrigen.CantidadDisponible -=
                        detalle.Cantidad;

                    inventarioOrigen.FechaActualizacion =
                        DateTime.Now;

                    var inventarioDestino =
                        await ObtenerOCrearInventario(
                            detalle.MaterialId,
                            dto.BodegaDestinoId);

                    inventarioDestino.CantidadDisponible +=
                        detalle.Cantidad;

                    inventarioDestino.FechaActualizacion =
                        DateTime.Now;

                    builder.AgregarDetalle(
                        detalle.MaterialId,
                        detalle.Cantidad);
                }

                var movimiento = builder.Build();

                _context.Movimientos.Add(movimiento);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new MovimientoResultadoDto
                {
                    MovimientoId = movimiento.Id,
                    TipoMovimiento = movimiento.TipoMovimiento,
                    Fecha = movimiento.Fecha,
                    Mensaje = "Traslado registrado correctamente."
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task ValidarMaterial(int materialId)
        {
            var existe =
                await _context.Materiales.AnyAsync(x =>
                    x.Id == materialId &&
                    x.Activo);

            if (!existe)
            {
                throw new InvalidOperationException(
                    $"El material {materialId} no existe o está inactivo.");
            }
        }

        private async Task ValidarBodega(int bodegaId)
        {
            var existe =
                await _context.Bodegas.AnyAsync(x =>
                    x.Id == bodegaId &&
                    x.Activo);

            if (!existe)
            {
                throw new InvalidOperationException(
                    $"La bodega {bodegaId} no existe o está inactiva.");
            }
        }

        private async Task<Models.Inventario>
            ObtenerOCrearInventario(
                int materialId,
                int bodegaId)
        {
            var inventario =
                await _context.Inventarios
                    .FirstOrDefaultAsync(x =>
                        x.MaterialId == materialId &&
                        x.BodegaId == bodegaId);

            if (inventario != null)
                return inventario;

            inventario =
                new Models.Inventario
                {
                    MaterialId = materialId,
                    BodegaId = bodegaId,
                    CantidadDisponible = 0,
                    FechaActualizacion = DateTime.Now
                };

            _context.Inventarios.Add(inventario);

            return inventario;
        }

        public async Task<MovimientoResultadoDto> RegistrarDevolucion(
    DevolucionDto dto)
        {
            await ValidarBodega(dto.BodegaDestinoId);

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var builder = new MovimientoBuilder()
                    .ConTipo("DEVOLUCION")
                    .ConUsuario(dto.UsuarioId)
                    .ConBodegaDestino(dto.BodegaDestinoId)
                    .ConSolicitud(dto.SolicitudId)
                    .ConObservacion(dto.Observacion);

                foreach (var detalle in dto.Detalles)
                {
                    await ValidarMaterial(detalle.MaterialId);

                    if (detalle.Cantidad <= 0)
                    {
                        throw new InvalidOperationException(
                            "La cantidad debe ser mayor que cero.");
                    }

                    var inventario =
                        await ObtenerOCrearInventario(
                            detalle.MaterialId,
                            dto.BodegaDestinoId);

                    inventario.CantidadDisponible +=
                        detalle.Cantidad;

                    inventario.FechaActualizacion =
                        DateTime.Now;

                    builder.AgregarDetalle(
                        detalle.MaterialId,
                        detalle.Cantidad);
                }

                var movimiento = builder.Build();

                _context.Movimientos.Add(movimiento);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new MovimientoResultadoDto
                {
                    MovimientoId = movimiento.Id,
                    TipoMovimiento = movimiento.TipoMovimiento,
                    Fecha = movimiento.Fecha,
                    Mensaje = "Devolución registrada correctamente."
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<MovimientoResultadoDto> RegistrarAjuste(
    AjusteDto dto)
        {
            await ValidarBodega(dto.BodegaId);

            var tipoAjuste = dto.TipoAjuste
                .Trim()
                .ToUpper();

            if (tipoAjuste != "POSITIVO" &&
                tipoAjuste != "NEGATIVO")
            {
                throw new InvalidOperationException(
                    "El tipo de ajuste debe ser POSITIVO o NEGATIVO.");
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var builder = new MovimientoBuilder()
                    .ConTipo($"AJUSTE_{tipoAjuste}")
                    .ConUsuario(dto.UsuarioId)
                    .ConObservacion(dto.Observacion);

                if (tipoAjuste == "POSITIVO")
                {
                    builder.ConBodegaDestino(dto.BodegaId);
                }
                else
                {
                    builder.ConBodegaOrigen(dto.BodegaId);
                }

                foreach (var detalle in dto.Detalles)
                {
                    await ValidarMaterial(detalle.MaterialId);

                    if (detalle.Cantidad <= 0)
                    {
                        throw new InvalidOperationException(
                            "La cantidad debe ser mayor que cero.");
                    }

                    if (tipoAjuste == "POSITIVO")
                    {
                        var inventario =
                            await ObtenerOCrearInventario(
                                detalle.MaterialId,
                                dto.BodegaId);

                        inventario.CantidadDisponible +=
                            detalle.Cantidad;

                        inventario.FechaActualizacion =
                            DateTime.Now;
                    }
                    else
                    {
                        var inventario =
                            await _context.Inventarios
                                .FirstOrDefaultAsync(x =>
                                    x.MaterialId ==
                                        detalle.MaterialId &&
                                    x.BodegaId ==
                                        dto.BodegaId);

                        if (inventario == null)
                        {
                            throw new InvalidOperationException(
                                $"El material {detalle.MaterialId} " +
                                "no tiene inventario en la bodega.");
                        }

                        if (inventario.CantidadDisponible <
                            detalle.Cantidad)
                        {
                            throw new InvalidOperationException(
                                $"Stock insuficiente para ajustar " +
                                $"el material {detalle.MaterialId}.");
                        }

                        inventario.CantidadDisponible -=
                            detalle.Cantidad;

                        inventario.FechaActualizacion =
                            DateTime.Now;
                    }

                    builder.AgregarDetalle(
                        detalle.MaterialId,
                        detalle.Cantidad);
                }

                var movimiento = builder.Build();

                _context.Movimientos.Add(movimiento);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new MovimientoResultadoDto
                {
                    MovimientoId = movimiento.Id,
                    TipoMovimiento = movimiento.TipoMovimiento,
                    Fecha = movimiento.Fecha,
                    Mensaje = "Ajuste registrado correctamente."
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<MovimientoConsultaDto>>
    ConsultarMovimientos(
        string? tipoMovimiento,
        int? materialId)
        {
            var query = _context.Movimientos
                .AsNoTracking()
                .Include(x => x.Detalles)
                    .ThenInclude(x => x.Material)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tipoMovimiento))
            {
                var tipo = tipoMovimiento.Trim().ToUpper();

                query = query.Where(
                    x => x.TipoMovimiento == tipo);
            }

            if (materialId.HasValue)
            {
                query = query.Where(
                    x => x.Detalles.Any(
                        d => d.MaterialId ==
                             materialId.Value));
            }

            return await query
                .OrderByDescending(x => x.Fecha)
                .Select(x => new MovimientoConsultaDto
                {
                    Id = x.Id,

                    TipoMovimiento =
                        x.TipoMovimiento,

                    Fecha = x.Fecha,

                    UsuarioId =
                        x.UsuarioId,

                    BodegaOrigenId =
                        x.BodegaOrigenId,

                    BodegaDestinoId =
                        x.BodegaDestinoId,

                    SolicitudId =
                        x.SolicitudId,

                    Observacion =
                        x.Observacion,

                    Detalles =
                        x.Detalles.Select(d =>
                            new MovimientoDetalleConsultaDto
                            {
                                MaterialId =
                                    d.MaterialId,

                                CodigoMaterial =
                                    d.Material != null
                                        ? d.Material.Codigo
                                        : string.Empty,

                                DescripcionMaterial =
                                    d.Material != null
                                        ? d.Material.Descripcion
                                        : string.Empty,

                                Cantidad =
                                    d.Cantidad
                            })
                            .ToList()
                })
                .ToListAsync();
        }

    }
}