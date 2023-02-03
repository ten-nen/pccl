
using System.ComponentModel.DataAnnotations;

namespace Pccl.ProjectTemplate.Application.Dtos
{

    public class UserLoginDto
    {
        [Required(ErrorMessage = "手机号不能为空")]
        [StringLength(32)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(32)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<RoleDto> UserRoles { get; set; }
    }

    public class UserPagedQueryDto : PagedQueryDto
    {
        public string? Filter { get; set; }
        public Guid? RoleId { get; set; }
    }

    public class UserQueryDto
    {
        public Guid? Id { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
    }

    public class UserCreateDto
    {
        [Required(ErrorMessage = "手机号不能为空")]
        [StringLength(32)]
        public string Phone { get; set; }
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(32)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string? Name { get; set; }
        public List<Guid> UserRoleIds { get; set; }
    }

    public class UserUpdateDto
    {
        [Required(ErrorMessage = "手机号不能为空")]
        [StringLength(32)]
        public string Phone { get; set; }
        [StringLength(32)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string? Name { get; set; }
        public List<Guid> UserRoleIds { get; set; }
    }
}
