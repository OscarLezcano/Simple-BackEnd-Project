using System.ComponentModel.DataAnnotations;
namespace BackEnd.Models;

public class Role
{
    [Key]
    //  0 -> Admin
    //  1 -> User
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    // Relationships 
    public virtual List<User>? Users { get; set; }
}