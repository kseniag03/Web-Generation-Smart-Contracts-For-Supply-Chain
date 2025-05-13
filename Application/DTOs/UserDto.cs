namespace Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Role { get; set; } = "user";
        public string? GitHubId { get; set; }
        public string? WalletAddress { get; set; }
    }
}
