using MediatR;
using GrubPix.Application.DTO;
using GrubPix.Domain.Interfaces.Repositories;
using AutoMapper;
using GrubPix.Application.Services.Interfaces;

namespace GrubPix.Application.Features.User
{
    public class GetUserByIdQuery : IRequest<BaseUserDto>
    {
        public int UserId { get; }

        public GetUserByIdQuery(int userId)
        {
            UserId = userId;
        }

        public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, BaseUserDto>
        {
            private readonly IUserService _userService;

            public GetUserByIdHandler(IUserService userService)
            {
                _userService = userService;
            }

            public async Task<BaseUserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
            {
                return await _userService.GetUserByIdAsync(request.UserId);
            }
        }
    }
}
