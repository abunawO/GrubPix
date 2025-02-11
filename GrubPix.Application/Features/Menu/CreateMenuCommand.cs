using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;
using MediatR;
using GrubPix.Application.Services.Interfaces;

namespace GrubPix.Application.Features.Menu
{
    // Command to create a new menu
    public class CreateMenuCommand : IRequest<MenuDto>
    {
        public CreateMenuDto MenuDto { get; set; }

        public CreateMenuCommand(CreateMenuDto menuDto)
        {
            MenuDto = menuDto;
        }
    }

    // Command Handler to process CreateMenuCommand
    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, MenuDto>
    {
        private readonly IMenuService _menuService;

        public CreateMenuCommandHandler(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<MenuDto> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            return await _menuService.CreateMenuAsync(request.MenuDto);
        }
    }
}
