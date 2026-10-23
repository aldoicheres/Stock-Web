namespace StockWeb.Models; 

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("USUARIOS")]
public class Usuario
{
    [Key]
    public long Id { get; set; }
    [Column("USUARIO")]
    public string USUARIO { get; set; } = string.Empty;
    [Column("EMAIL")]
    public string EMAIL { get; set; } = string.Empty;
    [Column("PASSWORDHASH")]
    public string PASSWORDHASH { get; set; } = string.Empty;
    [Column("ROL")]
    public string ROL { get; set; } = string.Empty;
    [Column("USUARIO_ID")]
    public string USUARIO_ID { get; set; } = string.Empty;
    [Column("ULTIMA_MODIFICACION")]
    public string ULTIMA_MODIFICACION { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
}