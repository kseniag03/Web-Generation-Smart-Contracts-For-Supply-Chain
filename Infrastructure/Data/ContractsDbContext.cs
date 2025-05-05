using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public partial class ContractsDbContext : DbContext
    {
        public ContractsDbContext()
        {
        }

        public ContractsDbContext(DbContextOptions<ContractsDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Actionlog> Actionlogs { get; set; }

        public virtual DbSet<Role> Roles { get; set; }

        public virtual DbSet<Smartcontract> Smartcontracts { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Userauth> Userauths { get; set; }

        public virtual DbSet<Wallet> Wallets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Actionlog>(entity =>
            {
                entity.HasKey(e => e.IdLog).HasName("actionlog_pkey");

                entity.ToTable("actionlog");

                entity.Property(e => e.IdLog).HasColumnName("id_log");
                entity.Property(e => e.ActionStatus)
                    .HasMaxLength(50)
                    .HasColumnName("action_status");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now() at time zone 'utc'")
                    .HasColumnName("created_at");
                entity.Property(e => e.Details).HasColumnName("details");
                entity.Property(e => e.IdContract).HasColumnName("id_contract");
                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.HasOne(d => d.IdContractNavigation).WithMany(p => p.Actionlogs)
                    .HasForeignKey(d => d.IdContract)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("actionlog_id_contract_fkey");

                entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Actionlogs)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("actionlog_id_user_fkey");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.IdRole);
                entity.Property(e => e.IdRole)
                    .UseIdentityAlwaysColumn();

                entity.HasKey(e => e.IdRole).HasName("roles_pkey");

                entity.ToTable("roles");

                entity.HasIndex(e => e.RoleName, "roles_role_name_key").IsUnique();

                entity.Property(e => e.IdRole).HasColumnName("id_role");
                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .HasColumnName("role_name");
            });

            modelBuilder.Entity<Smartcontract>(entity =>
            {
                entity.HasKey(e => e.IdContract).HasName("smartcontract_pkey");

                entity.ToTable("smartcontract");

                entity.Property(e => e.IdContract).HasColumnName("id_contract");
                entity.Property(e => e.ContractAddress)
                    .HasMaxLength(255)
                    .HasColumnName("contract_address");
                entity.Property(e => e.ContractData).HasColumnName("contract_data");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now() at time zone 'utc'")
                    .HasColumnName("created_at");
                entity.Property(e => e.GuidContract)
                    .HasDefaultValueSql("gen_random_uuid()")
                    .HasColumnName("guid_contract");
                entity.Property(e => e.IdUser).HasColumnName("id_user");
                entity.Property(e => e.IdWallet).HasColumnName("id_wallet");
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Smartcontracts)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("smartcontract_id_user_fkey");

                entity.HasOne(d => d.IdWalletNavigation).WithMany(p => p.Smartcontracts)
                    .HasForeignKey(d => d.IdWallet)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("smartcontract_id_wallet_fkey");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.IdUser);
                entity.Property(e => e.IdUser)
                    .UseIdentityAlwaysColumn();

                entity.HasKey(e => e.IdUser).HasName("users_pkey");

                entity.ToTable("users");

                entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

                entity.HasIndex(e => e.Login, "users_login_key").IsUnique();

                entity.Property(e => e.IdUser).HasColumnName("id_user");
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");
                entity.Property(e => e.Firstname)
                    .HasMaxLength(100)
                    .HasColumnName("firstname");
                entity.Property(e => e.Lastname)
                    .HasMaxLength(100)
                    .HasColumnName("lastname");
                entity.Property(e => e.Login)
                    .HasMaxLength(50)
                    .HasColumnName("login");
                entity.Property(e => e.GitHubId)
                    .HasColumnName("github_id");

                entity.HasMany(d => d.IdRoles).WithMany(p => p.IdUsers)
                    .UsingEntity<Dictionary<string, object>>(
                        "Userrole",
                        r => r.HasOne<Role>().WithMany()
                            .HasForeignKey("IdRole")
                            .HasConstraintName("userrole_id_role_fkey"),
                        l => l.HasOne<User>().WithMany()
                            .HasForeignKey("IdUser")
                            .HasConstraintName("userrole_id_user_fkey"),
                        j =>
                        {
                            j.HasKey("IdUser", "IdRole").HasName("userrole_pkey");
                            j.ToTable("userrole");
                            j.IndexerProperty<int>("IdUser").HasColumnName("id_user");
                            j.IndexerProperty<int>("IdRole").HasColumnName("id_role");
                        });
            });

            modelBuilder.Entity<Userauth>(entity =>
            {
                entity.HasKey(e => e.IdUser).HasName("userauth_pkey");

                entity.ToTable("userauth");

                entity.Property(e => e.IdUser)
                    .ValueGeneratedNever()
                    .HasColumnName("id_user");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");

                entity.HasOne(d => d.IdUserNavigation).WithOne(p => p.Userauth)
                    .HasForeignKey<Userauth>(d => d.IdUser)
                    .HasConstraintName("userauth_id_user_fkey");
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.HasKey(e => e.IdWallet);
                entity.Property(e => e.IdWallet)
                    .UseIdentityAlwaysColumn();

                entity.HasKey(e => e.IdWallet).HasName("wallet_pkey");

                entity.ToTable("wallet");

                entity.HasIndex(e => e.Address, "wallet_address_key").IsUnique();

                entity.Property(e => e.IdWallet).HasColumnName("id_wallet");
                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .HasColumnName("address");
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now() at time zone 'utc'")
                    .HasColumnName("created_at");
                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Wallets)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("wallet_id_user_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
