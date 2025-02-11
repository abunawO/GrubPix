using GrubPix.Application.DTO;
using GrubPix.Application.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GrubPix.Application.Features.Restaurant
{
    public class CreateRestaurantCommand : IRequest<RestaurantDto>
    {
        public CreateRestaurantDto RestaurantDto { get; set; }
        public IFormFile ImageFile { get; set; }

        public CreateRestaurantCommand(CreateRestaurantDto restaurantDto, IFormFile imageFile)
        {
            RestaurantDto = restaurantDto;
            ImageFile = imageFile;
        }
    }

    public class CreateRestaurantCommandHandler : IRequestHandler<CreateRestaurantCommand, RestaurantDto>
    {
        private readonly IRestaurantService _restaurantService;

        public CreateRestaurantCommandHandler(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        public async Task<RestaurantDto> Handle(CreateRestaurantCommand request, CancellationToken cancellationToken)
        {
            return await _restaurantService.CreateRestaurantAsync(request.RestaurantDto, request.ImageFile);
        }
    }
}
