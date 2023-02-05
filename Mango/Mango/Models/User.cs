using Microsoft.AspNetCore.Identity;
using NuGet.ContentModel;

namespace Mango.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
    }
}
