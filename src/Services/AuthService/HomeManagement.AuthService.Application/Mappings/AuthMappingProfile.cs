using AutoMapper;
using HomeManagement.AuthService.Application.DTOs;
using HomeManagement.AuthService.Domain.Entities;

namespace HomeManagement.AuthService.Application.Mapping
{
  public class AuthMappingProfile : Profile
  {
    public AuthMappingProfile()
    {
      CreateMap<User, UserDto>()
          .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

      CreateMap<RegisterDto, User>()
          .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
          .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

      // Add more mappings as needed
    }
  }
}