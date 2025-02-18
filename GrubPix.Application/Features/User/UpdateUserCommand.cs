using MediatR;
using GrubPix.Application.DTO;
using GrubPix.Domain.Interfaces.Repositories;
using AutoMapper;

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
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;

            public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
            {
                _userRepository = userRepository;
                _mapper = mapper;
            }

            public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetByIdAsync(request.Id);
                if (user == null)
                    return false; // User not found

                _mapper.Map(request.Dto, user); // Map DTO fields to User entity

                var updatedUser = await _userRepository.UpdateAsync(user);
                return updatedUser != null; // Return true if update was successful
            }
        }
    }
}
