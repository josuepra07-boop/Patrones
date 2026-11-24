using SolicitudesService.Prototypes;

namespace SolicitudesService.Models
{
    public class Solicitud : IPrototype<Solicitud>
    {
        public int Id { get; set; }

        public string UsuarioSolicitanteId { get; set; } = string.Empty;

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        public string Estado { get; set; } = "PENDIENTE";

        public string? Observacion { get; set; }

        public DateTime? FechaAprobacion { get; set; }

        public string? UsuarioAprobadorId { get; set; }

        public ICollection<SolicitudDetalle> Detalles { get; set; }
            = new List<SolicitudDetalle>();


        public Solicitud Clonar()
        {
            return new Solicitud
            {
                UsuarioSolicitanteId = UsuarioSolicitanteId,

                FechaSolicitud = DateTime.Now,

                Estado = "PENDIENTE",

                Observacion = Observacion,

                FechaAprobacion = null,

                UsuarioAprobadorId = null,

                Detalles = Detalles
                    .Select(x => new SolicitudDetalle
                    {
                        MaterialId = x.MaterialId,
                        CodigoMaterial = x.CodigoMaterial,
                        DescripcionMaterial = x.DescripcionMaterial,
                        CantidadSolicitada = x.CantidadSolicitada,
                        CantidadAprobada = null,
                        Observacion = x.Observacion
                    })
                    .ToList()
            };
        }
    }
}