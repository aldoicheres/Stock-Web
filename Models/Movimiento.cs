using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockWeb.Models
{
    [Table("MOVIMIENTOS")]
    public class Movimiento
    {
        [Key]
        public long Id { get; set; }

        [Column("PRODUCTOS_ID")]
        public long ProductosId { get; set; }

        [ForeignKey("ProductosId")]
        public Producto? Producto { get; set; }

        [Column("TIPO_MOVIMIENTO")]
        public string TipoMovimiento { get; set; } = "Ingreso"; // "Ingreso" ykf "Egreso"

        [Column("CANTIDAD")]
        public decimal Cantidad { get; set; }

        [Column("FECHARETIRO")]
        public string FechaRetiro { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        [Column("PERSONA_RETIRA")]
        public string PersonaRetira { get; set; } = string.Empty;

        [Column("USUARIO_ID")]
        public long UsuarioId { get; set; }

        [Column("OBSERVACIONES")]
        public string Observaciones { get; set; } = string.Empty;
    }
}