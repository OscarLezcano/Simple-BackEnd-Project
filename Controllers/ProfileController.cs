using Microsoft.AspNetCore.Mvc;
using BackEnd.Models.Response.User;
using BackEnd.Models.Response;
using BackEnd.Context;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using BackEnd.Models.Constants;

namespace BackEnd.Controllers;

[Route("api/profile")]
[ApiController]
[Authorize]
public class ProfileController(AppDbContext context, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    [HttpGet()]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID from JWT claims

        var profile = await _context.Users
            .Where(u => u.Id.ToString() == userId)
            .Include(u => u.Role)
            .Include(u => u.PhoneNumbers)
            .FirstOrDefaultAsync();

        if (profile == null)
        {
            string errorMessage = ApplicationError.NotFoundError.UserNotFound;
            return NotFound(new ApiResponseDto
            {
                Success = false,
                Message = errorMessage,
                Errors = new { User = new[] { errorMessage } }
            });
        }

        return Ok(new ApiResponseDto
        {
            Success = true,
            Message = ApplicationMessages.Success.UserProfileRetrieved,
            Data = _mapper.Map<UserResponseDto>(profile)
        });
    }
}