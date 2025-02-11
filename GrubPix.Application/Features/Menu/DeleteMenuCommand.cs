using MediatR;
using GrubPix.Application.Services.Interfaces;

namespace GrubPix.Application.Features.Menu
{
    public class DeleteMenuCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteMenuCommand(int id)
        {
            Id = id;
        }
    }

    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, bool>
    {
        private readonly IMenuService _menuService;

        public DeleteMenuCommandHandler(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<bool> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
            return await _menuService.DeleteMenuAsync(request.Id);
        }
    }
}
