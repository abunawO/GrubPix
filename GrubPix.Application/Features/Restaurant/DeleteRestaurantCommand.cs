using GrubPix.Application.Services.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GrubPix.Application.Features.Restaurant
{
    public class DeleteRestaurantCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteRestaurantCommand(int id)
        {
            Id = id;
        }
    }

    public class DeleteRestaurantCommandHandler : IRequestHandler<DeleteRestaurantCommand, bool>
    {
        private readonly IRestaurantService _restaurantService;

        public DeleteRestaurantCommandHandler(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        public async Task<bool> Handle(DeleteRestaurantCommand request, CancellationToken cancellationToken)
        {
            return await _restaurantService.DeleteRestaurantAsync(request.Id);
        }
    }
}
