using GrubPix.Application.DTO;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Domain.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GrubPix.Application.Features.Restaurant
{
    public class GetRestaurantByIdQuery : IRequest<RestaurantDto>
    {
        public int Id { get; set; }

        public GetRestaurantByIdQuery(int id)
        {
            Id = id;
        }
    }

    public class GetRestaurantByIdQueryHandler : IRequestHandler<GetRestaurantByIdQuery, RestaurantDto>
    {
        private readonly IRestaurantService _restaurantService;

        public GetRestaurantByIdQueryHandler(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        public async Task<RestaurantDto> Handle(GetRestaurantByIdQuery request, CancellationToken cancellationToken)
        {
            return await _restaurantService.GetRestaurantByIdAsync(request.Id);
        }
    }
}
