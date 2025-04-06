

using AutoMapper;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services
{
    public class LabelService : ILabelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LabelService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LabelDto>> GetAllAsync()
        {
            var labels = await _unitOfWork.Labels.GetAllAsync();
            return labels.Select(_mapper.Map<LabelDto>);
        }

        public async Task<LabelDto> CreateAsync(string name)
        {
            var label = new Label { Name = name };
            await _unitOfWork.Labels.AddAsync(label);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<LabelDto>(label);
        }
    }
}
