namespace Core.Entities;

public partial class Smartcontract
{
    public int IdContract { get; set; }

    public int? IdUser { get; set; }

    public int? IdWallet { get; set; }

    public Guid GuidContract { get; set; }

    public string? ContractData { get; set; }

    public string? ContractAddress { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Actionlog> Actionlogs { get; set; } = new List<Actionlog>();

    public virtual User? IdUserNavigation { get; set; }

    public virtual Wallet? IdWalletNavigation { get; set; }
}
