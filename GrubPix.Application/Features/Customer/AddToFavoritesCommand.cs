using GrubPix.Application.Interfaces;
using GrubPix.Application.Interfaces.Services;
using GrubPix.Domain.Entities;
using MediatR;

namespace GrubPix.Application.Features.Customer
{
    public class AddToFavoritesCommand : IRequest<bool>
    {
        public int CustomerId { get; }
        public int MenuItemId { get; }

        public AddToFavoritesCommand(int userId, int menuItemId)
        {
            CustomerId = userId;

            MenuItemId = menuItemId;
        }

        public class AddToFavoritesHandler : IRequestHandler<AddToFavoritesCommand, bool>
        {
            private readonly ICustomerService _customerService;

            public AddToFavoritesHandler(ICustomerService customerService)
            {
                _customerService = customerService;
            }

            public async Task<bool> Handle(AddToFavoritesCommand request, CancellationToken cancellationToken)
            {
                return await _customerService.AddFavoriteAsync(request.CustomerId, request.MenuItemId);
            }
        }

    }
}
