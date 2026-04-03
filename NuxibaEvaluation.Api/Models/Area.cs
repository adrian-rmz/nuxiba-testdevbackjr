using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NuxibaEvaluation.Api.Models;

[Table("ccRIACat_Areas")]
public class Area
{
    [Key]
    [Column("IDArea")]
    public int IDArea { get; set; }

    public string? AreaName { get; set; }

    public int? StatusArea { get; set; }

    public DateTime? CreateDate { get; set; }
}