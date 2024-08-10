using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests;

public class RegisterRequest
{
    [Required]
    [StringLength(50)]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }
    [Required]
    [StringLength(50)]
    public string LastName { get; set; }
    [Required]
    [StringLength(15)]
    public string Phone { get; set; }
    [Required]
    [StringLength(5)]
    public string PhoneRegion { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 5)]
    public string Password { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 5)]
    public string ConfirmPassword { get; set; }
}

public class LoginRequest
{
    [StringLength(50)]
    [EmailAddress]
    public string? Email { get; set; } = string.Empty;
    [StringLength(20)]
    public string? MobileNo { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 5)]
    public string Password { get; set; }
}

public class ResetPasswordRequest
{
    [Required]
    public string Token { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 5)]
    public string NewPassword { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 5)]
    public string ConfirmPassword { get; set; }
}