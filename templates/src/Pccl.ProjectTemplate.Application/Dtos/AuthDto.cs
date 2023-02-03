using System.ComponentModel.DataAnnotations;

namespace Pccl.ProjectTemplate.Application.Dtos
{
    public class UserAuthDto
    {
        public string token_type { get; set; } = "Bearer";
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
    }

    public class UserTokenDto
    {
        [Required]
        [StringLength(1024)]
        public string access_token { get; set; }
        [Required]
        [StringLength(128)]
        public string refresh_token { get; set; }
    }
}
