using MediatR;
using GrubPix.Application.DTOs;
using GrubPix.Application.Interfaces;
using GrubPix.Application.Interfaces.Services;
using GrubPix.Application.Exceptions;

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
            private readonly ICustomerService _customerService;

            public GetCustomerProfileHandler(ICustomerService customerService)
            {
                _customerService = customerService;
            }

            public async Task<CustomerDto> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
            {
                var customer = await _customerService.GetCustomerByIdAsync(request.CustomerId);
                return customer ?? throw new NotFoundException($"Customer with ID {request.CustomerId} not found.");
            }
        }
    }
}
