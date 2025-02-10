using MediatR;
using GrubPix.Application.DTO;
using GrubPix.Application.Services.Interfaces;

namespace GrubPix.Application.Features.MenuItem
{
    public class CreateMenuItemCommand : IRequest<MenuItemDto>
    {
        public CreateMenuItemDto MenuItemDto { get; }

        public CreateMenuItemCommand(CreateMenuItemDto menuItemDto)
        {
            MenuItemDto = menuItemDto;
        }
    }

    public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, MenuItemDto>
    {
        private readonly IMenuItemService _menuItemService;

        public CreateMenuItemCommandHandler(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        public async Task<MenuItemDto> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
        {
            return await _menuItemService.CreateMenuItemAsync(request.MenuItemDto);
        }
    }
}
