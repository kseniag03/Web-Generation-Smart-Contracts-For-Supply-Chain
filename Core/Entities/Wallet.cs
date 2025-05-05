namespace Core.Entities;

public partial class Wallet
{
    public int IdWallet { get; set; }

    public int? IdUser { get; set; }

    public string Address { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }

    public virtual User? IdUserNavigation { get; set; }

    public virtual ICollection<Smartcontract> Smartcontracts { get; set; } = new List<Smartcontract>();
}
