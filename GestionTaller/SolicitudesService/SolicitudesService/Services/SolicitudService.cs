using Microsoft.EntityFrameworkCore;
using SolicitudesService.Data;
using SolicitudesService.DTOs;
using SolicitudesService.Interfaces;
using SolicitudesService.Models;
using SolicitudesService.Events;
using SolicitudesService.Messaging;

namespace SolicitudesService.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly SolicitudesDbContext _context;
        private readonly IRabbitMqPublisher _publisher;

        public SolicitudService(
            SolicitudesDbContext context,
            IRabbitMqPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }

        public async Task<List<SolicitudDto>> ConsultarTodos(
            string? estado)
        {
            var query = _context.Solicitudes
                .AsNoTracking()
                .Include(x => x.Detalles)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(estado))
            {
                var estadoFiltro =
                    estado.Trim().ToUpper();

                query = query.Where(
                    x => x.Estado == estadoFiltro);
            }

            var solicitudes =
                await query
                    .OrderByDescending(x => x.FechaSolicitud)
                    .ToListAsync();

            return solicitudes
                .Select(MapearDto)
                .ToList();
        }

        public async Task<SolicitudDto?> ConsultarPorId(
            int id)
        {
            var solicitud =
                await _context.Solicitudes
                    .AsNoTracking()
                    .Include(x => x.Detalles)
                    .FirstOrDefaultAsync(
                        x => x.Id == id);

            if (solicitud == null)
                return null;

            return MapearDto(solicitud);
        }

        public async Task<SolicitudDto> Crear(
            CrearSolicitudDto dto)
        {
            if (dto.Detalles.Count == 0)
            {
                throw new InvalidOperationException(
                    "La solicitud debe contener al menos un material.");
            }

            var solicitud = new Solicitud
            {
                UsuarioSolicitanteId =
                    dto.UsuarioSolicitanteId,

                FechaSolicitud =
                    DateTime.Now,

                Estado =
                    "PENDIENTE",

                Observacion =
                    dto.Observacion
            };

            foreach (var detalle in dto.Detalles)
            {
                if (detalle.Cantidad <= 0)
                {
                    throw new InvalidOperationException(
                        $"La cantidad del material " +
                        $"{detalle.MaterialId} debe ser mayor que cero.");
                }

                solicitud.Detalles.Add(
                    new SolicitudDetalle
                    {
                        MaterialId =
                            detalle.MaterialId,

                        CodigoMaterial =
                            detalle.CodigoMaterial.Trim(),

                        DescripcionMaterial =
                            detalle.DescripcionMaterial.Trim(),

                        CantidadSolicitada =
                            detalle.Cantidad,

                        Observacion =
                            detalle.Observacion
                    });
            }

            _context.Solicitudes.Add(solicitud);

            await _context.SaveChangesAsync();

            return MapearDto(solicitud);
        }

        public async Task<SolicitudDto> Clonar(
            int id,
            ClonarSolicitudDto dto)
        {
            var original =
                await _context.Solicitudes
                    .AsNoTracking()
                    .Include(x => x.Detalles)
                    .FirstOrDefaultAsync(
                        x => x.Id == id);

            if (original == null)
            {
                throw new InvalidOperationException(
                    "La solicitud original no existe.");
            }

            // PATRÓN PROTOTYPE
            var nuevaSolicitud =
                original.Clonar();

            nuevaSolicitud.UsuarioSolicitanteId =
                dto.UsuarioSolicitanteId;

            if (!string.IsNullOrWhiteSpace(
                dto.Observacion))
            {
                nuevaSolicitud.Observacion =
                    dto.Observacion;
            }

            _context.Solicitudes.Add(
                nuevaSolicitud);

            await _context.SaveChangesAsync();

            return MapearDto(nuevaSolicitud);
        }

        public async Task<bool> Aprobar(
            int id,
            AprobarSolicitudDto dto)
        {
            var solicitud =
                await _context.Solicitudes
                    .Include(x => x.Detalles)
                    .FirstOrDefaultAsync(
                        x => x.Id == id);

            if (solicitud == null)
                return false;

            if (solicitud.Estado != "PENDIENTE")
            {
                throw new InvalidOperationException(
                    "Solo se pueden aprobar solicitudes pendientes.");
            }

            foreach (var detalle
                in solicitud.Detalles)
            {
                var aprobacion =
                    dto.Detalles.FirstOrDefault(
                        x => x.DetalleId ==
                             detalle.Id);

                if (aprobacion != null)
                {
                    if (aprobacion.CantidadAprobada < 0)
                    {
                        throw new InvalidOperationException(
                            "La cantidad aprobada no puede ser negativa.");
                    }

                    if (aprobacion.CantidadAprobada >
                        detalle.CantidadSolicitada)
                    {
                        throw new InvalidOperationException(
                            $"La cantidad aprobada del material " +
                            $"{detalle.CodigoMaterial} supera " +
                            "la cantidad solicitada.");
                    }

                    detalle.CantidadAprobada =
                        aprobacion.CantidadAprobada;
                }
                else
                {
                    // Si no se especifica,
                    // se aprueba toda la cantidad.
                    detalle.CantidadAprobada =
                        detalle.CantidadSolicitada;
                }
            }

            var estadoAnterior =
                solicitud.Estado;

            solicitud.Estado =
                "APROBADA";

            solicitud.FechaAprobacion =
                DateTime.Now;

            solicitud.UsuarioAprobadorId =
                dto.UsuarioAprobadorId;

            await _context.SaveChangesAsync();

            var evento = new SolicitudAprobadaEvent
            {
                SolicitudId = solicitud.Id,

                BodegaId = dto.BodegaId,

                UsuarioAprobadorId =
        dto.UsuarioAprobadorId,

                Fecha = DateTime.Now,

                Detalles = solicitud.Detalles
        .Where(x =>
            x.CantidadAprobada.HasValue &&
            x.CantidadAprobada.Value > 0)
        .Select(x =>
            new SolicitudAprobadaDetalleEvent
            {
                MaterialId = x.MaterialId,

                Cantidad =
                    x.CantidadAprobada!.Value
            })
        .ToList()
            };

            await _publisher.PublicarAsync(
                "solicitud.aprobada",
                evento);

            return true;
        }

        public async Task<bool> Rechazar(
            int id,
            RechazarSolicitudDto dto)
        {
            var solicitud =
                await _context.Solicitudes
                    .FirstOrDefaultAsync(
                        x => x.Id == id);

            if (solicitud == null)
                return false;

            if (solicitud.Estado != "PENDIENTE")
            {
                throw new InvalidOperationException(
                    "Solo se pueden rechazar solicitudes pendientes.");
            }

            var estadoAnterior =
                solicitud.Estado;

            solicitud.Estado =
                "RECHAZADA";

            await _context.SaveChangesAsync();

            return true;
        }

        private static SolicitudDto MapearDto(
            Solicitud solicitud)
        {
            return new SolicitudDto
            {
                Id =
                    solicitud.Id,

                UsuarioSolicitanteId =
                    solicitud.UsuarioSolicitanteId,

                FechaSolicitud =
                    solicitud.FechaSolicitud,

                Estado =
                    solicitud.Estado,

                Observacion =
                    solicitud.Observacion,

                FechaAprobacion =
                    solicitud.FechaAprobacion,

                UsuarioAprobadorId =
                    solicitud.UsuarioAprobadorId,

                Detalles =
                    solicitud.Detalles
                        .Select(x =>
                            new SolicitudDetalleDto
                            {
                                Id =
                                    x.Id,

                                MaterialId =
                                    x.MaterialId,

                                CodigoMaterial =
                                    x.CodigoMaterial,

                                DescripcionMaterial =
                                    x.DescripcionMaterial,

                                CantidadSolicitada =
                                    x.CantidadSolicitada,

                                CantidadAprobada =
                                    x.CantidadAprobada,

                                Observacion =
                                    x.Observacion
                            })
                        .ToList(),

            };
        }

        public async Task<bool> MarcarAtendida(
    int solicitudId,
    int movimientoId)
        {
            var solicitud =
                await _context.Solicitudes
                    .FirstOrDefaultAsync(
                        x => x.Id == solicitudId);

            if (solicitud == null)
                return false;

            // Idempotencia básica:
            // si RabbitMQ entrega otra vez el mensaje,
            // no hacemos nuevamente el cambio.
            if (solicitud.Estado == "ATENDIDA")
                return true;

            if (solicitud.Estado != "APROBADA")
            {
                throw new InvalidOperationException(
                    $"La solicitud {solicitudId} " +
                    $"se encuentra en estado {solicitud.Estado}.");
            }

            var estadoAnterior =
                solicitud.Estado;

            solicitud.Estado =
                "ATENDIDA";

            await _context.SaveChangesAsync();

            return true;
        }
    }
}