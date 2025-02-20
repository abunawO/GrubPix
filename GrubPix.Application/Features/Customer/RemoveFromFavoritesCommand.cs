using GrubPix.Application.Interfaces;
using GrubPix.Application.Interfaces.Services;
using MediatR;

namespace GrubPix.Application.Features.Customer
{
    public class RemoveFromFavoritesCommand : IRequest<bool>
    {
        public int CustomerId { get; }
        public int MenuItemId { get; }

        public RemoveFromFavoritesCommand(int userId, int menuItemId)
        {
            CustomerId = userId;
            MenuItemId = menuItemId;
        }

        public class RemoveFromFavoritesHandler : IRequestHandler<RemoveFromFavoritesCommand, bool>
        {
            private readonly ICustomerService _customerService;

            public RemoveFromFavoritesHandler(ICustomerService customerService)
            {
                _customerService = customerService;
            }

            public async Task<bool> Handle(RemoveFromFavoritesCommand request, CancellationToken cancellationToken)
            {
                return await _customerService.RemoveFavoriteAsync(request.CustomerId, request.MenuItemId);
            }
        }

    }
}
