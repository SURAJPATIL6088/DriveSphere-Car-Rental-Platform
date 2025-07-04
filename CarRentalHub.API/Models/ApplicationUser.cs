using Microsoft.AspNetCore.Identity;

namespace CarRentalHub.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
