using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
namespace BackEnd.Models;

[Index(nameof(Email), IsUnique = true)]
public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    // Unique
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public int RoleId { get; set; } = 1;
    // Relationships 
    public virtual List<PhoneNumber>? PhoneNumbers { get; set; }
    public virtual Role? Role { get; set; }
}
