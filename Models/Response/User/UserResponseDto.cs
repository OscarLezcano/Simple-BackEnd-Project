namespace BackEnd.Models.Response.User;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public List<string> PhoneNumbers { get; set; } = [];
    public string RoleName { get; set; } = null!;
}

