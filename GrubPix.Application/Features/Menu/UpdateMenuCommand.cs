using AutoMapper;
using GrubPix.Application.DTO;
using GrubPix.Domain.Entities;
using GrubPix.Domain.Interfaces.Repositories;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace GrubPix.Application.Features.Menu
{
    public class UpdateMenuCommand : IRequest<MenuDto>
    {
        public int Id { get; set; }
        public CreateMenuDto MenuDto { get; set; }
    }

    public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, MenuDto>
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMapper _mapper;

        public UpdateMenuCommandHandler(IMenuRepository menuRepository, IMapper mapper)
        {
            _menuRepository = menuRepository;
            _mapper = mapper;
        }

        public async Task<MenuDto> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
        {
            var menuEntity = await _menuRepository.GetByIdAsync(request.Id);

            if (menuEntity == null)
                return null;

            _mapper.Map(request.MenuDto, menuEntity);

            await _menuRepository.UpdateAsync(menuEntity);

            return _mapper.Map<MenuDto>(menuEntity);
        }
    }
}
