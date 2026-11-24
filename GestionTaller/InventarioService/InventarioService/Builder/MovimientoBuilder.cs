using InventarioService.Models;

namespace InventarioService.Builders
{
    public class MovimientoBuilder
    {
        private readonly Movimiento _movimiento;

        public MovimientoBuilder()
        {
            _movimiento = new Movimiento
            {
                Fecha = DateTime.Now,
                Detalles = new List<MovimientoDetalle>()
            };
        }

        public MovimientoBuilder ConTipo(string tipo)
        {
            _movimiento.TipoMovimiento = tipo.ToUpper();
            return this;
        }

        public MovimientoBuilder ConUsuario(string usuarioId)
        {
            _movimiento.UsuarioId = usuarioId;
            return this;
        }

        public MovimientoBuilder ConBodegaOrigen(int? bodegaId)
        {
            _movimiento.BodegaOrigenId = bodegaId;
            return this;
        }

        public MovimientoBuilder ConBodegaDestino(int? bodegaId)
        {
            _movimiento.BodegaDestinoId = bodegaId;
            return this;
        }

        public MovimientoBuilder ConSolicitud(int? solicitudId)
        {
            _movimiento.SolicitudId = solicitudId;
            return this;
        }

        public MovimientoBuilder ConObservacion(string? observacion)
        {
            _movimiento.Observacion = observacion;
            return this;
        }

        public MovimientoBuilder AgregarDetalle(
            int materialId,
            decimal cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException(
                    "La cantidad debe ser mayor que cero.");

            _movimiento.Detalles.Add(
                new MovimientoDetalle
                {
                    MaterialId = materialId,
                    Cantidad = cantidad
                });

            return this;
        }

        public Movimiento Build()
        {
            if (string.IsNullOrWhiteSpace(_movimiento.TipoMovimiento))
                throw new InvalidOperationException(
                    "El tipo de movimiento es obligatorio.");

            if (string.IsNullOrWhiteSpace(_movimiento.UsuarioId))
                throw new InvalidOperationException(
                    "El usuario es obligatorio.");

            if (_movimiento.Detalles.Count == 0)
                throw new InvalidOperationException(
                    "El movimiento debe contener materiales.");

            return _movimiento;
        }
    }
}