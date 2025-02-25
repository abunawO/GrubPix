using GrubPix.Application.Common;
using GrubPix.Application.Services.Interfaces;
using MediatR;

namespace GrubPix.Application.Features.MenuItem
{
    public class DeleteMenuItemImageCommand : IRequest<ApiResponse<object>>
    {
        public int MenuItemId { get; }
        public int ImageId { get; }

        public DeleteMenuItemImageCommand(int menuItemId, int imageId)
        {
            MenuItemId = menuItemId;
            ImageId = imageId;
        }

        public class DeleteMenuItemImageHandler : IRequestHandler<DeleteMenuItemImageCommand, ApiResponse<object>>
        {
            private readonly IMenuItemService _menuItemService;

            public DeleteMenuItemImageHandler(IMenuItemService menuItemService)
            {
                _menuItemService = menuItemService;
            }

            public async Task<ApiResponse<object>> Handle(DeleteMenuItemImageCommand request, CancellationToken cancellationToken)
            {
                return await _menuItemService.DeleteMenuItemImageAsync(request.MenuItemId, request.ImageId);
            }
        }
    }
}
