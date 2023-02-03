using AutoMapper;
using Pccl.ProjectTemplate.Application.Dtos;
using Pccl.ProjectTemplate.Domain.Entities.PermissionAggregate;
using Pccl.ProjectTemplate.Domain.Entities.RoleAggregate;
using Pccl.ProjectTemplate.Domain.Entities.UserAggregate;

namespace Pccl.ProjectTemplate.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();

            CreateMap<Role, RoleDto>();

            CreateMap<Permission, PermissionDto>();
        }
    }
}