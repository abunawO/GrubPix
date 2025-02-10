using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;
using MediatR;

namespace GrubPix.Application.Features.Menu
{
    // Command to create a new menu
    public class CreateMenuCommand : IRequest<MenuDto>
    {
        public CreateMenuDto MenuDto { get; set; }
    }

    // Command Handler to process CreateMenuCommand
    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, MenuDto>
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;

        public CreateMenuCommandHandler(IMenuRepository menuRepository, IMapper mapper)
        {
            _menuRepository = menuRepository;
            _mapper = mapper;
        }

        public async Task<MenuDto> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            // Mapping DTO to Entity
            var menuEntity = _mapper.Map<GrubPix.Domain.Entities.Menu>(request.MenuDto);



            // Saving to the repository
            var createdMenu = await _menuRepository.AddAsync(menuEntity);

            // Mapping back Entity to DTO to return
            return _mapper.Map<MenuDto>(createdMenu);
        }
    }
}
