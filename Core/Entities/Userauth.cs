namespace Core.Entities;

public partial class Userauth
{
    public int IdUser { get; set; }

    public string PasswordHash { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
