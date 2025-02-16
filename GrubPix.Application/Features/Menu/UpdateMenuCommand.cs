using GrubPix.Application.DTO;
using GrubPix.Application.Services.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GrubPix.Application.Features.Menu
{
    public class UpdateMenuCommand : IRequest<MenuDto>
    {
        public int Id { get; set; }
        public UpdateMenuDto MenuDto { get; set; }

        public UpdateMenuCommand(int id, UpdateMenuDto menuDto)
        {
            Id = id;
            MenuDto = menuDto;
        }
    }

    public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, MenuDto>
    {
        private readonly IMenuService _menuService;

        public UpdateMenuCommandHandler(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<MenuDto> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
        {
            return await _menuService.UpdateMenuAsync(request.Id, request.MenuDto);
        }
    }
}
