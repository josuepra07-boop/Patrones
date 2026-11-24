using InventarioService.DTOs;
using InventarioService.Interfaces;
using InventarioService.Patterns.Observer;

namespace InventarioService.Patterns.Command
{
    public class CrearMaterialCommand : ICommand<CrearMaterialCommandResultadoDto>
    {
        private readonly IMaterialService _materialService;
        private readonly MaterialCreadoSubject _subject;
        private readonly CrearMaterialDto _dto;

        public CrearMaterialCommand(
            IMaterialService materialService,
            MaterialCreadoSubject subject,
            CrearMaterialDto dto)
        {
            _materialService = materialService;
            _subject = subject;
            _dto = dto;
        }

        public async Task<CrearMaterialCommandResultadoDto> ExecuteAsync()
        {
            // Receiver: MaterialService ejecuta la lógica real del negocio.
            var material = await _materialService.Crear(_dto);

            // Observer: una vez creado el material, se notifican los observadores.
            var notificaciones = await _subject.NotificarAsync(material);

            return new CrearMaterialCommandResultadoDto
            {
                Id = material.Id,
                Codigo = material.Codigo,
                Descripcion = material.Descripcion,
                CategoriaId = material.CategoriaId,
                Categoria = material.Categoria,
                UnidadMedida = material.UnidadMedida,
                StockMinimo = material.StockMinimo,
                Activo = material.Activo,
                PatronCommand = nameof(CrearMaterialCommand),
                Invoker = nameof(CommandInvoker),
                ObserversNotificados = notificaciones
            };
        }
    }
}
