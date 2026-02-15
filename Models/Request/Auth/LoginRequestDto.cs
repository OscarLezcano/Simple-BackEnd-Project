using System.ComponentModel.DataAnnotations;
using BackEnd.Models.Constants;

namespace BackEnd.Models.Request.Auth;

public class LoginRequestDto
{
    [Required(ErrorMessage = ApplicationError.RequiredField.EmailRequired)]
    [EmailAddress(ErrorMessage = ApplicationError.ValidationError.InvalidEmail)]
    public string Email { get; set; } = null!;
    [Required(ErrorMessage = ApplicationError.RequiredField.PasswordRequired)]
    public string Password { get; set; } = null!;
}