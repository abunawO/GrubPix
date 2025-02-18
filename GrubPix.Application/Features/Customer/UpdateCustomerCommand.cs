using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Application.Interfaces;
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
            private readonly ICustomerRepository _customerRepository;
            private readonly IMapper _mapper;

            public UpdateCustomerHandler(ICustomerRepository customerRepository, IMapper mapper)
            {
                _customerRepository = customerRepository;
                _mapper = mapper;
            }

            public async Task<bool> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
            {
                var customer = await _customerRepository.GetCustomerByIdAsync(request.Id);
                if (customer == null)
                {
                    return false; // Customer not found
                }

                // Update customer properties based on the request
                _mapper.Map(request.Dto, customer);
                // Add any other fields that need updating

                var updatedCustomer = await _customerRepository.UpdateAsync(customer);

                return updatedCustomer != null; // Return true if update was successful
            }
        }
    }
}
