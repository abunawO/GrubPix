using GrubPix.Application.Interfaces;
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
            private readonly ICustomerRepository _customerRepository;

            public AddToFavoritesHandler(ICustomerRepository customerRepository)
            {
                _customerRepository = customerRepository;
            }

            public async Task<bool> Handle(AddToFavoritesCommand request, CancellationToken cancellationToken)
            {
                var customer = await _customerRepository.GetCustomerByIdAsync(request.CustomerId);
                if (customer == null) return false;

                var favoriteItem = new FavoriteMenuItem
                {
                    CustomerId = request.CustomerId,
                    MenuItemId = request.MenuItemId
                };

                customer.FavoriteMenuItems.Add(favoriteItem);
                await _customerRepository.UpdateAsync(customer);

                return true;
            }
        }

    }
}
