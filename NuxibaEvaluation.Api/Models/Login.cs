using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuxibaEvaluation.Api.Models;

[Table("ccloglogin")]
public class Login
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Column("User_id")]
    public int UserId { get; set; }

    [Required]
    public int Extension { get; set; }

    [Required]
    public int TipoMov { get; set; } // 1 = login, 0 = logout

    [Required]
    [Column("fecha")]
    public DateTime Fecha { get; set; }
}