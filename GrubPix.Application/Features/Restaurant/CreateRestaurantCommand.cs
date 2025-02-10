using MediatR;
using GrubPix.Application.DTO;
using GrubPix.Domain.Interfaces.Repositories;
using AutoMapper;
using RestaurantEntity = GrubPix.Domain.Entities.Restaurant;

namespace GrubPix.Application.Features.Restaurant
{
    public class CreateRestaurantCommand : IRequest<RestaurantDto>
    {
        public CreateRestaurantDto RestaurantDto { get; set; }

        public CreateRestaurantCommand(CreateRestaurantDto restaurantDto)
        {
            RestaurantDto = restaurantDto;
        }
    }

    public class CreateRestaurantCommandHandler : IRequestHandler<CreateRestaurantCommand, RestaurantDto>
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMapper _mapper;

        public CreateRestaurantCommandHandler(IRestaurantRepository restaurantRepository, IMapper mapper)
        {
            _restaurantRepository = restaurantRepository;
            _mapper = mapper;
        }

        public async Task<RestaurantDto> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
        {
            var restaurantEntity = _mapper.Map<RestaurantEntity>(request.RestaurantDto);
            var createdRestaurant = await _restaurantRepository.AddAsync(restaurantEntity);
            return _mapper.Map<RestaurantDto>(createdRestaurant);
        }
    }
}
