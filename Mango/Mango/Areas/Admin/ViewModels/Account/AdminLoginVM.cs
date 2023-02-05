using System.ComponentModel.DataAnnotations;

namespace Mango.Areas.Admin.ViewModels.Account
{
    public class AdminLoginVM
    {
        [Required]
        [StringLength(maximumLength: 30)]
        public string Username { get; set; }
        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
