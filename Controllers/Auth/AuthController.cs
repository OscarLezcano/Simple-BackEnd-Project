using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEnd.Models;
using BackEnd.Models.Request.Auth;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using BackEnd.Context;
using BackEnd.Models.Response.User;
using AutoMapper;
using BackEnd.Models.Response;
using BackEnd.Models.Constants;

namespace BackEnd.Controllers.Auth;

[Route("api/auth")]
[ApiController]
public class AuthController(AppDbContext context, IConfiguration config, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly IConfiguration _config = config;
    private readonly IMapper _mapper = mapper;
    private readonly int _tokenExpirationDays = 30;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new ApiResponseDto
            {
                Success = false,
                Message = ApplicationError.ValidationError.ValidationFailed,
                Errors = new { Email = new[] { ApplicationError.EmailError.EmailAlreadyExists } }
            });
        }


        var user = new User
        {
            Name = request.Name,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new ApiResponseDto
        {
            Success = true,
            Message = ApplicationMessages.Authentication.UserRegistered,
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        var user = await _context.Users
            .Include(u => u.PhoneNumbers)
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            string errorMessage = ApplicationError.ValidationError.InvalidCredentials;
            return BadRequest(new ApiResponseDto
            {
                Success = false,
                Message = errorMessage,
                Errors = new { Authentication = new[] { errorMessage } }
            });
        }
        // 1. Create token JWT
        string token = CreateToken(user);

        // 2. Save token in cookie
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.Now.AddDays(_tokenExpirationDays)
        };

        Response.Cookies.Append("current_user", token, cookieOptions);

        return Ok(new ApiResponseDto
        {
            Success = true,
            Message = ApplicationMessages.Authentication.LoginSuccessful,
            Data = _mapper.Map<UserResponseDto>(user)
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // Borrar la cookie
        Response.Cookies.Delete("current_user");
        return Ok(new ApiResponseDto
        {
            Success = true,
            Message = ApplicationMessages.Authentication.UserLoggedOut
        });
    }


    /** Funtion to create JWT token with user claims
    * Claims is a list of user information that will be included in 
    * the token, such as user id, email, name, last name and role
    */
    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Name),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.Role, user.Role!.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddDays(_tokenExpirationDays),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}