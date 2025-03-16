namespace Core.Entities;

public partial class Role
{
    public int IdRole { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<User> IdUsers { get; set; } = new List<User>();
}
