using AutoMapper;
using Pccl.AutoDI;
using Pccl.Repository;
using Pccl.ProjectTemplate.Application.Dtos;
using Pccl.ProjectTemplate.Application.Interfaces;
using Pccl.ProjectTemplate.Domain.Interfaces;

namespace Pccl.ProjectTemplate.Application.Services
{
    public class PermissionService : IPermissionService, IScopedService
    {
        protected readonly IMapper Mapper;
        protected IUnitOfWork UnitOfWork;
        protected readonly IPermissionRepository PermissionRepository;
        public PermissionService(IMapper mapper, IUnitOfWork unitOfWork, IPermissionRepository permissionRepository)
        {
            Mapper = mapper;
            UnitOfWork = unitOfWork;
            PermissionRepository = permissionRepository;
        }

        public virtual async Task<IEnumerable<PermissionDto>> GetAllAsync()
        {
            var list = await PermissionRepository.GetAllAsync();
            return Mapper.Map<IEnumerable<PermissionDto>>(list);
        }
    }
}
