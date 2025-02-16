using AutoMapper;
using GrubPix.Domain.Interfaces.Repositories;
using GrubPix.Application.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using GrubPix.Application.DTO;

namespace GrubPix.Application.Features.Restaurant
{
    public class UpdateRestaurantCommand : IRequest<RestaurantDto>
    {
        public int Id { get; set; }
        public UpdateRestaurantDto RestaurantDto { get; set; }
        public IFormFile ImageFile { get; set; }

        public UpdateRestaurantCommand(int id, UpdateRestaurantDto restaurantDto, IFormFile imageFile)
        {
            Id = id;
            RestaurantDto = restaurantDto;
            ImageFile = imageFile;
        }
    }

    public class UpdateRestaurantCommandHandler : IRequestHandler<UpdateRestaurantCommand, RestaurantDto>
    {
        private readonly IRestaurantService _restaurantService;

        public UpdateRestaurantCommandHandler(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        public async Task<RestaurantDto> Handle(UpdateRestaurantCommand request, CancellationToken cancellationToken)
        {
            return await _restaurantService.UpdateRestaurantAsync(request.Id, request.RestaurantDto, request.ImageFile);
        }
    }
}
