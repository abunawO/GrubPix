using MediatR;
using GrubPix.Application.DTO;
using GrubPix.Domain.Interfaces.Repositories;
using AutoMapper;
using GrubPix.Application.Services.Interfaces;

namespace GrubPix.Application.Features.User
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public int Id { get; }
        public UpdateUserDto Dto { get; }

        public UpdateUserCommand(int id, UpdateUserDto dto)
        {
            Id = id;
            Dto = dto;
        }

        public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
        {
            private readonly IUserService _userService;
            private readonly IMapper _mapper;

            public UpdateUserCommandHandler(IUserService userService, IMapper mapper)
            {
                _userService = userService;
                _mapper = mapper;
            }

            public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                var updatedUser = await _userService.UpdateUserAsync(request.Id, request.Dto);
                return updatedUser != null;
            }
        }
    }
}
