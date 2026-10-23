using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockWeb.Models
{
    [Table("DETALLES_REMITO")]
    public class DetalleRemito
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("RemitoId")]
        public long RemitoId { get; set; }

        [Column("ProductosId")]
        public long ProductosId { get; set; }

        [Column("Cantidad")]
        public double Cantidad { get; set; }

        [Column("USUARIO_ID")]
        public long UsuarioId { get; set; }

        [Column("ULTIMA_MODIFICACION")]
        public string UltimaModificacion { get; set; } = string.Empty;

        // Propiedades de navegación para Entity Framework Core
        [ForeignKey("RemitoId")]
        public Remito? Remito { get; set; }

        [ForeignKey("ProductosId")]
        public Producto? Producto { get; set; }
    }
}