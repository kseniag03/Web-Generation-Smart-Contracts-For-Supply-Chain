namespace Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public required string Login { get; set; }
        public string? Email { get; set; }
        public required string Role { get; set; }
        public string? GitHubId { get; set; }
        public string? WalletAddress { get; set; }
    }
}
