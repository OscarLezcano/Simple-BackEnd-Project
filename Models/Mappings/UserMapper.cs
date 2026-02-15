using BackEnd.Models.Response.User;

namespace BackEnd.Models.Mappings;

public class UserMapper : AutoMapper.Profile
{
    public UserMapper()
    {
        // Basic case: Property with the same name and type maps automatically
        // Advanced case: Map Role.Name to DTO RoleName
        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.RoleName,
                       opt => opt.MapFrom(src => src.Role!.Name))
            .ForMember(dest => dest.PhoneNumbers,
                       opt => opt.MapFrom(src => src.PhoneNumbers!.Select(p => p.Number)));
    }
}