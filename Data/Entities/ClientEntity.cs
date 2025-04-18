using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class ClientEntity
{
    [Key]
    public int Id { get; set; }

    public string ClientName { get; set; } = null!;
}
