using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace StockWeb.Models
{
    [Table("REMITOS")]
    public class Remito
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("FECHA")]
        public string Fecha { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        [Column("TIPOMOVIMIENTO")]
        public string TipoMovimiento { get; set; } = "Ingreso"; // "Ingreso" o "Egreso"

        [Column("PERSONARETIRA")]
        public string PersonaRetira { get; set; } = string.Empty;

        [Column("OBSERVACIONES")]
        public string? Observaciones { get; set; }

        [Column("USUARIO_ID")]
        public long UsuarioId { get; set; }

        [Column("ULTIMA_MODIFICACION")]
        public string UltimaModificacion { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // Relación Master-Detail (1 Remito -> Muchos Detalles)
        public List<DetalleRemito> Detalles { get; set; } = new List<DetalleRemito>();
    }
}