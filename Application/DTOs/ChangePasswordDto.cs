using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password required")]
        [MinLength(6, ErrorMessage = "Current password must be at least 6 characters")]
        [RegularExpression(@"^[A-Za-z0-9!@#$%\^&\*\(\)_\+\-=.,\?]+$",
        ErrorMessage = "Current password can contain only Latin letters, numbers, and symbols: ! @ # $ % ^ & * ( ) _ + - = . , ?")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password required")]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters")]
        [RegularExpression(@"^[A-Za-z0-9!@#$%\^&\*\(\)_\+\-=.,\?]+$",
        ErrorMessage = "New password can contain only Latin letters, numbers, and symbols: ! @ # $ % ^ & * ( ) _ + - = . , ?")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
