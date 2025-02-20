using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Application.Interfaces;
using GrubPix.Application.Interfaces.Services;
using MediatR;

namespace GrubPix.Application.Features.Customer
{
    public class UpdateCustomerCommand : IRequest<bool>
    {
        private string? Email;
        private string? PasswordHash;
        public UpdateCustomerDto Dto { get; }
        public int Id { get; private set; }

        public UpdateCustomerCommand(int userId, UpdateCustomerDto dto)
        {
            Id = userId;
            Dto = dto;
        }

        public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, bool>
        {
            private readonly ICustomerService _customerService;
            private readonly IMapper _mapper;

            public UpdateCustomerHandler(ICustomerService customerService, IMapper mapper)
            {
                _customerService = customerService;
                _mapper = mapper;
            }

            public async Task<bool> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
            {
                var updatedCustomer = await _customerService.UpdateAsync(request.Id, request.Dto);
                return updatedCustomer != null;
            }
        }

    }
}
