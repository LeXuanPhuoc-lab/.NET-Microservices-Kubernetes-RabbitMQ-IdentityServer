using System.ComponentModel.DataAnnotations;

namespace IdentityService;

public class RegisterViewModel
{
    [Required]
    public string Email { get; set; } = string.Empty;    

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Fullname { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
    public string Button { get; set; } = string.Empty;
}