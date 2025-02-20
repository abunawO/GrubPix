using MediatR;
using System.Collections.Generic;
using GrubPix.Application.DTOs;
using GrubPix.Application.DTO;
using GrubPix.Application.Interfaces;
using GrubPix.Application.Interfaces.Services;

namespace GrubPix.Application.Features.Customer
{
    public class GetCustomerFavoritesQuery : IRequest<List<MenuItemDto>>
    {
        public int CustomerId { get; }

        public GetCustomerFavoritesQuery(int userId)
        {
            CustomerId = userId;
        }

        public class GetCustomerFavoritesHandler : IRequestHandler<GetCustomerFavoritesQuery, List<MenuItemDto>>
        {
            private readonly ICustomerService _customerService;

            public GetCustomerFavoritesHandler(ICustomerService customerService)
            {
                _customerService = customerService;
            }

            public async Task<List<MenuItemDto>> Handle(GetCustomerFavoritesQuery request, CancellationToken cancellationToken)
            {
                return await _customerService.GetFavoriteMenuItemsAsync(request.CustomerId);
            }
        }

    }
}
