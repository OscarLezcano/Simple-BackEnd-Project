using System.ComponentModel.DataAnnotations;
using BackEnd.Models.Constants;

namespace BackEnd.Models.Request.Auth;

public class RegisterRequestDto
{
    [Required(ErrorMessage = ApplicationError.RequiredField.NameRequired)]
    public string Name { get; set; } = null!;
    [Required(ErrorMessage = ApplicationError.RequiredField.LastNameRequired)]
    public string LastName { get; set; } = null!;
    [Required(ErrorMessage = ApplicationError.RequiredField.EmailRequired)]
    [EmailAddress(ErrorMessage = ApplicationError.ValidationError.InvalidEmail)]
    public string Email { get; set; } = null!;
    [Required(ErrorMessage = ApplicationError.RequiredField.PasswordRequired)]
    [MinLength(8, ErrorMessage = ApplicationError.LengthError.PasswordLength)]
    public string Password { get; set; } = null!;
}