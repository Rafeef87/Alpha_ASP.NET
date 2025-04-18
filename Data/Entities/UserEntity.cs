using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class UserEntity : IdentityUser
{
    public string? ProfileImage { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; } 
    public string? JobTitle { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
