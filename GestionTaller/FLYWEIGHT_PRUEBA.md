# Prueba Flyweight con movimientos reales de SQL Server

Los endpoints no reciben body, parámetros ni filtros.

## SIN Flyweight

```http
GET /api/Patrones/flyweight/sin
```

Consulta todos los registros de `MovimientoDetalles` junto con sus datos de
`Movimientos`, `Materiales` y `Categorias`.

Después de terminar la consulta SQL se inicia la medición. Cada detalle crea
una instancia independiente de `MaterialSinFlyweight`.

La respuesta devuelve:

- cantidad total de registros procesados;
- cantidad de materiales diferentes;
- tiempo de procesamiento en memoria;
- memoria asignada;
- cantidad de objetos Material creados;
- todos los movimientos procesados;
- identificador de la instancia Material usada por cada registro.

## CON Flyweight

```http
GET /api/Patrones/flyweight/con
```

Consulta exactamente la misma fuente SQL. Después utiliza
`MaterialFlyweightFactory` para compartir una única instancia de
`MaterialFlyweight` por `MaterialId`.

En la respuesta, varios movimientos con el mismo `MaterialId` deben mostrar
el mismo `InstanciaMaterialEnMemoria`.

## Importante

La consulta SQL y la serialización JSON se realizan fuera de la medición del
patrón. El tiempo y memoria reportados corresponden únicamente a la creación y
procesamiento de objetos en memoria.
