namespace Core.Entities
{/*
    [Table("users")] // Указываем имя таблицы в БД
    public class UserOld
    {
        [Key]
        [Column("id_user")] // Синхронизируем с PostgreSQL
        public int Id { get; set; }  // Заменить int → Guid
        // public Guid Id { get; set; } // Уникальный ID

        [Required]
        [Column("login")]
        public string? Username { get; set; } // Имя

        [Column("email")]
        public string? Email { get; set; } // Email (если нужно)

        [Required]
        [Column("id_role")]
        public RoleType Role { get; set; } // Роль (enum)

        // AspNetCore.Identity
        [NotMapped]
        public string? PasswordHash { get; set; } // хэш пароля

        // OAuth
        [NotMapped]
        public string? WalletAddress { get; set; } // Привязанный MetaMask

        [NotMapped]
        public string? GitHubId { get; set; } // Привязанный GitHub (если есть)
    }*/
}
