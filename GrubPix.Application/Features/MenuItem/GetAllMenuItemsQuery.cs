using MediatR;
using System.Collections.Generic;
using GrubPix.Application.DTO;
using GrubPix.Application.Services.Interfaces;

namespace GrubPix.Application.Features.MenuItem
{
    public class GetAllMenuItemsQuery : IRequest<List<MenuItemDto>> { }

    public class GetAllMenuItemsQueryHandler : IRequestHandler<GetAllMenuItemsQuery, List<MenuItemDto>>
    {
        private readonly IMenuItemService _menuItemService;

        public GetAllMenuItemsQueryHandler(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        public async Task<List<MenuItemDto>> Handle(GetAllMenuItemsQuery request, CancellationToken cancellationToken)
        {
            return (await _menuItemService.GetAllMenuItemsAsync()).ToList();
        }
    }
}