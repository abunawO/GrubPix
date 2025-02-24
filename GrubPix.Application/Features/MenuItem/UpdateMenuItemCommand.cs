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
        public ICollection<IFormFile> ImageFiles { get; }

        public UpdateMenuItemCommand(int id, UpdateMenuItemDto menuItemDto, ICollection<IFormFile> imageFiles)
        {
            Id = id;
            MenuItemDto = menuItemDto;
            ImageFiles = imageFiles;
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
            return await _menuItemService.UpdateMenuItemAsync(request.Id, request.MenuItemDto, request.ImageFiles);
        }
    }
}
