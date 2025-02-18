using GrubPix.Application.Interfaces;
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
            private readonly ICustomerRepository _customerRepository;

            public RemoveFromFavoritesHandler(ICustomerRepository customerRepository)
            {
                _customerRepository = customerRepository;
            }

            public async Task<bool> Handle(RemoveFromFavoritesCommand request, CancellationToken cancellationToken)
            {
                var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);
                if (customer == null)
                {
                    return false; // Customer not found
                }

                var favoriteItem = customer.FavoriteMenuItems.FirstOrDefault(fm => fm.MenuItemId == request.MenuItemId);
                if (favoriteItem == null)
                {
                    return false; // Menu item is not in favorites
                }

                customer.FavoriteMenuItems.Remove(favoriteItem);
                await _customerRepository.UpdateAsync(customer);

                return true; // Successfully removed
            }
        }
    }
}
