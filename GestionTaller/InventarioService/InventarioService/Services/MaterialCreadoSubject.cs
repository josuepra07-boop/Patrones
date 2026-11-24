using InventarioService.DTOs;

namespace InventarioService.Patterns.Observer
{
    public class MaterialCreadoSubject
    {
        private readonly List<IMaterialCreadoObserver> _observers;

        public MaterialCreadoSubject(IEnumerable<IMaterialCreadoObserver> observers)
        {
            _observers = observers.ToList();
        }

        public void Suscribir(IMaterialCreadoObserver observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
        }

        public void Desuscribir(IMaterialCreadoObserver observer)
        {
            _observers.Remove(observer);
        }

        public async Task<List<ObserverNotificacionDto>> NotificarAsync(MaterialDto material)
        {
            var resultados = new List<ObserverNotificacionDto>();

            foreach (var observer in _observers)
            {
                try
                {
                    resultados.Add(await observer.ActualizarAsync(material));
                }
                catch (Exception ex)
                {
                    // Un observer no debe revertir una creación que ya fue confirmada en SQL Server.
                    resultados.Add(new ObserverNotificacionDto
                    {
                        Observer = observer.Nombre,
                        Mensaje = $"El observer no pudo procesar la notificación: {ex.Message}"
                    });
                }
            }

            return resultados;
        }
    }
}
