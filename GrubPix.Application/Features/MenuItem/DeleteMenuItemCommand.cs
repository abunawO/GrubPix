using GrubPix.Application.Services.Interfaces;
using MediatR;

namespace GrubPix.Application.Features.MenuItem
{
    public class DeleteMenuItemCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteMenuItemCommand(int id)
        {
            Id = id;
        }
    }

    public class DeleteMenuItemCommandHandler : IRequestHandler<DeleteMenuItemCommand, bool>
    {
        private readonly IMenuItemService _menuItemService;

        public DeleteMenuItemCommandHandler(IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        public async Task<bool> Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
        {
            return await _menuItemService.DeleteMenuItemAsync(request.Id);
        }
    }
}
