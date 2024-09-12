using AutoMapper;
using MediatR;
using HomeManagement.AuthService.Application.DTOs;
using HomeManagement.AuthService.Domain.Interfaces;

namespace HomeManagement.AuthService.Application.Queries
{
  public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
  {
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
      _userRepository = userRepository;
      _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
      var user = await _userRepository.GetByIdAsync(request.UserId);
      return _mapper.Map<UserDto>(user);
    }
  }
}