using Microsoft.AspNetCore.Mvc;
using BackEnd.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BackEnd.Models.Response.User;
using AutoMapper;
using BackEnd.Models.Response;
using BackEnd.Models.Constants;

namespace BackEnd.Controllers.Admin;

[Route("api/admin/users")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UserController(AppDbContext context, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    [HttpGet()]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.PhoneNumbers)
            .ToListAsync();

        return Ok(new ApiResponseDto
        {
            Success = true,
            Message = ApplicationMessages.Success.UsersRetrieved,
            Data = _mapper.Map<List<UserResponseDto>>(users)
        });
    }
}