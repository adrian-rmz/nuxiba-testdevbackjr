using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuxibaEvaluation.Api.Models;

[Table("ccUsers")]
public class User
{
    [Key]
    [Column("User_id")]
    public int UserId { get; set; }

    [Required]
    public string Login { get; set; } = string.Empty;

    public string? Nombres { get; set; }
    public string? ApellidoPaterno { get; set; }
    public string? ApellidoMaterno { get; set; }
    public string? Password { get; set; }

    [Column("TipoUser_id")]
    public int? TipoUserId { get; set; }

    public int? Status { get; set; }

    public DateTime? fCreate { get; set; }

    [Column("IDArea")]
    public int? IDArea { get; set; }

    public DateTime? LastLoginAttempt { get; set; }
}