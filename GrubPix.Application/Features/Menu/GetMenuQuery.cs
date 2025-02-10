using AutoMapper;
using GrubPix.Application.DTO;
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

        public GetMenuQueryHandler(IMenuRepository menuRepository, IMapper mapper)
        {
            _menuRepository = menuRepository;
            _mapper = mapper;
        }

        public async Task<List<MenuDto>> Handle(GetMenuQuery request, CancellationToken cancellationToken)
        {
            var menus = await _menuRepository.GetAllAsync();
            return _mapper.Map<List<MenuDto>>(menus);
        }
    }
}
