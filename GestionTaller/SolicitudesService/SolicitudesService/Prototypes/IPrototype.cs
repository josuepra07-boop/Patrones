namespace SolicitudesService.Prototypes
{
    public interface IPrototype<T>
    {
        T Clonar();
    }
}