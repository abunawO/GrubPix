using MediatR;
using System.Collections.Generic;
using GrubPix.Application.DTOs;
using GrubPix.Application.DTO;
using GrubPix.Application.Interfaces;

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
            private readonly ICustomerRepository _customerRepository;

            public GetCustomerFavoritesHandler(ICustomerRepository customerRepository)
            {
                _customerRepository = customerRepository;
            }

            public async Task<List<MenuItemDto>> Handle(GetCustomerFavoritesQuery request, CancellationToken cancellationToken)
            {
                var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);
                return customer?.FavoriteMenuItems
                    .Select(fm => new MenuItemDto
                    {
                        Id = fm.MenuItem.Id,
                        Name = fm.MenuItem.Name,
                        Description = fm.MenuItem.Description,
                        Price = fm.MenuItem.Price,
                        MenuId = fm.MenuItem.MenuId,
                        ImageUrl = fm.MenuItem.ImageUrl
                    }).ToList();
            }
        }
    }
}
