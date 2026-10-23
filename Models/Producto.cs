using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockWeb.Models
{
    [Table("PRODUCTOS")]
    public class Producto
    {
        [Key]
        public long Id { get; set; }

        [Column("NOMBRE")]
        public string Nombre { get; set; } = string.Empty;

        [Column("DESCRIPCION")]
        public string Descripcion { get; set; } = string.Empty;

        [Column("STOCKACTUAL")]
        public decimal StockActual { get; set; }

        [Column("CATEGORIAS_ID")]
        public long CategoriasId { get; set; }

        [ForeignKey("CategoriasId")]
        public Categoria? Categoria { get; set; }

        [Column("USUARIO_ID")]
        public string UsuarioId { get; set; } = string.Empty;

        [Column("ULTIMA_MODIFICACION")]
        public string UltimaModificacion { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}