using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class MetaMaskDto
    {
        [Required(ErrorMessage = "WalletAddress is required")]
        [RegularExpression(@"^0x[a-fA-F0-9]{40}$",
            ErrorMessage = "WalletAddress must start with '0x' and be followed by 40 hex characters (0–9, A–F).")]
        public required string WalletAddress { get; set; }
    }
}
