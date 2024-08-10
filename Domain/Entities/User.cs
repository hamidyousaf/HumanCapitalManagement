using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        [MaxLength(30)]
        public string FirstName { get; set; }
        [MaxLength(30)]
        public string LastName { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOnUtc { get; set; }
        public DateTime? DeletedOnUtc { get; set; }
        public short? OTP { get; set; }
        public bool IsGuestAccount { get; set; } = false;
        [MaxLength(20)]
        public override string? PhoneNumber { get; set; }
        public DateTime? OTPSentTime { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
