using Microsoft.AspNetCore.Identity;


namespace SCCGasso.Infrastructure.Identity.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsActive { get; set; }
        public string? LastLogin { get; set; }
        public string? IdCard { get; set; }

        public int IdRoleAppPermission { get; set; }
    }
}
