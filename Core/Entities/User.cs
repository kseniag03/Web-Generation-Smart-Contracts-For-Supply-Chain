using Core.Enums;

namespace Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string WalletAddress { get; set; }
        public RoleEnum Role { get; set; }
    }
}
