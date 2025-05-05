namespace Core.Entities;

public partial class Actionlog
{
    public int IdLog { get; set; }

    public int? IdUser { get; set; }

    public int? IdContract { get; set; }

    public string? ActionStatus { get; set; }

    public string? Details { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public virtual Smartcontract? IdContractNavigation { get; set; }

    public virtual User? IdUserNavigation { get; set; }
}
