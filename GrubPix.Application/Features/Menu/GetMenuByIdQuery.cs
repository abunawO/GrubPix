using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Application.Services.Interfaces;
using GrubPix.Domain.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GrubPix.Application.Features.Menu
{
    public class GetMenuByIdQuery : IRequest<MenuDto>
    {
        public int Id { get; set; }

        public GetMenuByIdQuery(int id)
        {
            Id = id;
        }
    }

    public class GetMenuByIdQueryHandler : IRequestHandler<GetMenuByIdQuery, MenuDto>
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;
        private readonly IMenuService _menuService;


        public GetMenuByIdQueryHandler(IMenuService menuService, IMapper mapper)
        {
            _menuService = menuService;
            _mapper = mapper;
        }

        public async Task<MenuDto> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
        {
            return await _menuService.GetMenuByIdAsync(request.Id);
        }
    }
}
