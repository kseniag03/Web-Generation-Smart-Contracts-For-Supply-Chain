namespace Application.DTOs
{
    public class RegisterDto
    {
        public required string Login { get; set; }
        public required string Password { get; set; }
        public string? Email { get; set; }
    }
}
