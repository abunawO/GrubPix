using MediatR;
using GrubPix.Application.DTO;
using GrubPix.Application.Services.Interfaces;

namespace GrubPix.Application.Features.MenuItem
{
    public class GetMenuItemByIdQuery : IRequest<MenuItemDto>
    {
        public int Id { get; }

        public GetMenuItemByIdQuery(int id)
        {
            Id = id;
        }
    }

    public class GetMenuItemByIdQueryHandler : IRequestHandler<GetMenuItemByIdQuery, MenuItemDto>
    {
        private readonly IMenuItemService _menuItemService;

        public GetMenuItemByIdQueryHandler(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        public async Task<MenuItemDto> Handle(GetMenuItemByIdQuery request, CancellationToken cancellationToken)
        {
            return await _menuItemService.GetMenuItemByIdAsync(request.Id);
        }
    }
}
