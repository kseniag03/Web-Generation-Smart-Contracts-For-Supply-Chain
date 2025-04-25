namespace Core.Entities;

public partial class User
{
    public int IdUser { get; set; }

    public string Login { get; set; } = null!;

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? Email { get; set; }

    public string? GitHubId { get; set; }

    public virtual ICollection<Actionlog> Actionlogs { get; set; } = new List<Actionlog>();

    public virtual ICollection<Smartcontract> Smartcontracts { get; set; } = new List<Smartcontract>();

    public virtual Userauth? Userauth { get; set; }

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();

    public virtual ICollection<Role> IdRoles { get; set; } = new List<Role>();
}
