using Microsoft.AspNetCore.Identity;

namespace Payroll.Infrastructure.Identity.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
    }
}
