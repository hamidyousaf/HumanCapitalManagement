using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class Roles : IdentityRole
    {
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}
