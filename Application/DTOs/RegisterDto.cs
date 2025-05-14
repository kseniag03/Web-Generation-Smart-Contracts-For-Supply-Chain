using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Login required")]
        [RegularExpression(@"^[A-Za-z0-9_]+$",
            ErrorMessage = "Login: only Latin letters, numbers and '_'.")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password required")]
        [MinLength(6, ErrorMessage = "The password must be at least 6 characters")]
        [RegularExpression(@"^[A-Za-z0-9!@#$%\^&\*\(\)_\+\-=.,\?]+$",
        ErrorMessage = "The password can contain only Latin letters, numbers, and symbols: ! @ # $ % ^ & * ( ) _ + - = . , ?")]
        public string Password { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Incorrect email format")]
        public string? Email { get; set; }
    }
}
