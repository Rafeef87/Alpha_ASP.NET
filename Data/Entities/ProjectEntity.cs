
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ProjectEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? ProjectImage { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public string? ClientName { get; set; } = null!;
    public string Status { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime EndDate { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Budget { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Updated { get; set; } = DateTime.Now;
    public ICollection<MemberEntity> projectMembers { get; set; } = [];

}
