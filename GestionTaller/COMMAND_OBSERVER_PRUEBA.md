# Command + Observer - GestiónTaller

## Caso implementado
La creación de un material usa dos patrones de comportamiento:

- **Command**: encapsula la petición de crear un material en `CrearMaterialCommand`.
- **Observer**: después de crear exitosamente el material, `MaterialCreadoSubject` notifica a los observers registrados.

## Endpoint

`POST /api/Materiales`

El endpoint y el JSON de entrada no cambian.

Ejemplo:

```json
{
  "codigo": "CMD001",
  "descripcion": "Material prueba Command Observer",
  "categoriaId": 1,
  "unidadMedida": "UND",
  "stockMinimo": 5
}
```

## Flujo

1. `MaterialesController` recibe el JSON.
2. El controller crea un `CrearMaterialCommand`.
3. `CommandInvoker` ejecuta el comando.
4. `CrearMaterialCommand` llama al receiver `MaterialService.Crear()`.
5. `MaterialService` valida categoría y guarda el material en SQL Server.
6. El comando llama a `MaterialCreadoSubject.NotificarAsync()`.
7. El Subject notifica a:
   - `AuditoriaMaterialObserver`
   - `LogMaterialObserver`
8. La API devuelve el material creado y las notificaciones ejecutadas.

## Roles Command

- Client: `MaterialesController`
- Command: `ICommand<TResult>`
- ConcreteCommand: `CrearMaterialCommand`
- Invoker: `CommandInvoker`
- Receiver: `MaterialService`

## Roles Observer

- Subject: `MaterialCreadoSubject`
- Observer: `IMaterialCreadoObserver`
- ConcreteObserver: `AuditoriaMaterialObserver`
- ConcreteObserver: `LogMaterialObserver`

## Verificación

La respuesta debe incluir campos como:

```json
{
  "id": 123,
  "codigo": "CMD001",
  "descripcion": "Material prueba Command Observer",
  "categoriaId": 1,
  "categoria": "...",
  "unidadMedida": "UND",
  "stockMinimo": 5,
  "activo": true,
  "patronCommand": "CrearMaterialCommand",
  "invoker": "CommandInvoker",
  "observersNotificados": [
    {
      "observer": "AuditoriaMaterialObserver",
      "mensaje": "Auditoría: material CMD001 creado con Id 123."
    },
    {
      "observer": "LogMaterialObserver",
      "mensaje": "Log: se procesó correctamente la creación de CMD001 - Material prueba Command Observer."
    }
  ]
}
```

También puede verificarse el material directamente en SQL Server:

```sql
SELECT *
FROM Materiales
WHERE Codigo = 'CMD001';
```
