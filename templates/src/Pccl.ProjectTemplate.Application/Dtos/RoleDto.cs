
using System.ComponentModel.DataAnnotations;

namespace Pccl.ProjectTemplate.Application.Dtos
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public List<PermissionDto> RolePermissions { get; set; }
    }

    public class RoleCreateDto
    {
        [Required]
        [StringLength(32)]
        public string Name { get; set; }
        public List<string> RolePermissionNames { get; set; }
    }

    public class RoleUpdateDto
    {
        [Required]
        [StringLength(32)]
        public string Name { get; set; }
        public List<string> RolePermissionNames { get; set; }
    }
}
