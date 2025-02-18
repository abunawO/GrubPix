using MediatR;
using GrubPix.Application.DTOs;
using GrubPix.Application.Interfaces;

namespace GrubPix.Application.Features.Customer
{
    public class GetCustomerProfileQuery : IRequest<CustomerDto>
    {
        public int CustomerId { get; }

        public GetCustomerProfileQuery(int userId)
        {
            CustomerId = userId;
        }

        public class GetCustomerProfileHandler : IRequestHandler<GetCustomerProfileQuery, CustomerDto>
        {
            private readonly ICustomerRepository _customerRepository;

            public GetCustomerProfileHandler(ICustomerRepository customerRepository)
            {
                _customerRepository = customerRepository;
            }

            public async Task<CustomerDto> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
            {
                var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);
                return customer != null ? new CustomerDto { Id = customer.Id, Email = customer.Email, Username = customer.Username } : null;
            }
        }
    }
}
