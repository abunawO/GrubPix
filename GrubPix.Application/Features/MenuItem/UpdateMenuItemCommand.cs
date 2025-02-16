using MediatR;
using Microsoft.AspNetCore.Http;
using GrubPix.Application.DTO;
using GrubPix.Application.Services.Interfaces;

namespace GrubPix.Application.Features.MenuItem
{
    public class UpdateMenuItemCommand : IRequest<MenuItemDto>
    {
        public int Id { get; }
        public UpdateMenuItemDto MenuItemDto { get; }
        public IFormFile ImageFile { get; }

        public UpdateMenuItemCommand(int id, UpdateMenuItemDto menuItemDto, IFormFile imageFile)
        {
            Id = id;
            MenuItemDto = menuItemDto;
            ImageFile = imageFile;
        }
    }

    public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand, MenuItemDto>
    {
        private readonly IMenuItemService _menuItemService;

        public UpdateMenuItemCommandHandler(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        public async Task<MenuItemDto> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
        {
            return await _menuItemService.UpdateMenuItemAsync(request.Id, request.MenuItemDto, request.ImageFile);
        }
    }
}
