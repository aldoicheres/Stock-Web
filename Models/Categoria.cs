using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockWeb.Models
{
    [Table("CATEGORIAS")]
    public class Categoria
    {
        [Key]
        public long Id { get; set; }

        [Column("NOMBRE")]
        public string Nombre { get; set; } = string.Empty;

        [Column("DESCRIPCION")]
        public string Descripcion { get; set; } = string.Empty;

        [Column("USUARIO_ID")]
        public string UsuarioId { get; set; } = string.Empty;

        [Column("ULTIMA_MODIFICACION")]
        public string UltimaModificacion { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    }
}