using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Domain.Interfaces.Repositories;
using MediatR;

namespace GrubPix.Application.Features.Restaurant
{
    public class GetRestaurantsQuery : IRequest<List<RestaurantDto>>
    {
        public string? Name { get; }
        public string? SortBy { get; }
        public bool Descending { get; }
        public int Page { get; }
        public int PageSize { get; }

        public GetRestaurantsQuery(string? name, string? sortBy, bool descending, int page, int pageSize)
        {
            Name = name;
            SortBy = sortBy;
            Descending = descending;
            Page = page;
            PageSize = pageSize;
        }
    }

    public class GetRestaurantsQueryHandler : IRequestHandler<GetRestaurantsQuery, List<RestaurantDto>>
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMapper _mapper;
        private readonly IRestaurantService _restaurantService;

        public GetRestaurantsQueryHandler(IRestaurantService restaurantService, IMapper mapper)
        {
            _restaurantService = restaurantService;
            _mapper = mapper;
        }

        public async Task<List<RestaurantDto>> Handle(GetRestaurantsQuery request, CancellationToken cancellationToken)
        {
            var restaurants = await _restaurantService.GetAllRestaurantsAsync(
                request.Name, request.SortBy, request.Descending, request.Page, request.PageSize
            );
            return restaurants.ToList();
        }
    }
}
