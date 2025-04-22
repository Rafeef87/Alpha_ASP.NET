
namespace Domain.Models;

public class Notification
{
    public int Id { get; set; }
    public string? Type { get; set; } 
    public string? Message { get; set; }
    public DateTime Time { get; set; }
}
