namespace InventarioService.DTOs
{
    /// <summary>
    /// Resultado individual de una ejecución CON o SIN Flyweight.
    /// Los datos procesados provienen de MovimientoDetalles reales de DB_INVENTARIO.
    /// </summary>
    public class FlyweightPruebaResultadoDto
    {
        public string TipoPrueba { get; set; } = string.Empty;
        public string OrigenDatos { get; set; } = "SQL Server - DB_INVENTARIO";
        public int RegistrosProcesados { get; set; }
        public int MaterialesDiferentes { get; set; }
        public double TiempoProcesamientoMs { get; set; }
        public long MemoriaAsignadaBytes { get; set; }
        public double MemoriaAsignadaMb { get; set; }
        public int ObjetosMaterialCreados { get; set; }

        /// <summary>
        /// Todos los MovimientoDetalles reales procesados en la prueba.
        /// </summary>
        public List<MovimientoRealDto> Movimientos { get; set; } = new();

        public string Interpretacion { get; set; } = string.Empty;

        public string NotaMedicion { get; set; } =
            "Los movimientos se consultan primero desde SQL Server. El tiempo y la memoria " +
            "mostrados miden únicamente la construcción/procesamiento de objetos en memoria; " +
            "la consulta SQL y la serialización JSON no se incluyen en la medición.";
    }

    /// <summary>
    /// MovimientoDetalle real consultado desde DB_INVENTARIO y representado
    /// posteriormente con o sin Flyweight.
    /// </summary>
    public class MovimientoRealDto
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

        /// <summary>
        /// Identificador de la instancia del objeto Material en memoria.
        /// CON Flyweight, los registros con el mismo MaterialId comparten este valor.
        /// SIN Flyweight, cada detalle tendrá normalmente un valor distinto.
        /// </summary>
        public int InstanciaMaterialEnMemoria { get; set; }
    }
}
