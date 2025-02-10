using MediatR;
using GrubPix.Application.DTO;
using GrubPix.Domain.Interfaces.Repositories;
using AutoMapper;

namespace GrubPix.Application.Features.Restaurant
{
    public class UpdateRestaurantCommand : IRequest<RestaurantDto>
    {
        public int Id { get; set; }
        public CreateRestaurantDto RestaurantDto { get; set; }

        public UpdateRestaurantCommand(int id, CreateRestaurantDto restaurantDto)
        {
            Id = id;
            RestaurantDto = restaurantDto;
        }
    }

    public class UpdateRestaurantCommandHandler : IRequestHandler<UpdateRestaurantCommand, RestaurantDto>
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMapper _mapper;

        public UpdateRestaurantCommandHandler(IRestaurantRepository restaurantRepository, IMapper mapper)
        {
            _restaurantRepository = restaurantRepository;
            _mapper = mapper;
        }

        public async Task<RestaurantDto> Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(request.Id);
            if (restaurant == null)
                return null;

            _mapper.Map(request.RestaurantDto, restaurant);
            await _restaurantRepository.UpdateAsync(restaurant);

            return _mapper.Map<RestaurantDto>(restaurant);
        }
    }
}