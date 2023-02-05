using System.ComponentModel.DataAnnotations;

namespace Mango.ViewModels.Account
{
    public class AccountLoginVM
    {
        [Required]
        public string Username { get; set; }

        [Required, MaxLength(100), DataType(DataType.Password)]
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
