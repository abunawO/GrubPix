using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Domain.Interfaces.Repositories;
using MediatR;

namespace GrubPix.Application.Features.Menu
{
    // Query to get the list of menu items
    public class GetMenuQuery : IRequest<List<MenuDto>> { }

    // Handler for processing the GetMenuQuery
    public class GetMenuQueryHandler : IRequestHandler<GetMenuQuery, List<MenuDto>>
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;
        private readonly IMenuService _menuService;

        public GetMenuQueryHandler(IMenuService menuService, IMapper mapper)
        {
            _menuService = menuService;
            _mapper = mapper;
        }

        public async Task<List<MenuDto>> Handle(GetMenuQuery request, CancellationToken cancellationToken)
        {
            return (await _menuService.GetMenusAsync()).ToList();
        }
    }
}
