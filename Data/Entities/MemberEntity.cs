namespace Data.Entities;

public class MemberEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string UserId { get; set; } = null!;
    public string ProjectId { get; set; } = null!;
    public ProjectEntity Project { get; set; } = null!;
    public UserEntity User { get; set; } = null!;

}
