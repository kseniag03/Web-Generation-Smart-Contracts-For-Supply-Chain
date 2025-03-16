using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Core.Entities;

namespace Infrastructure.Data
{/*
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Role).HasConversion<string>(); // Enum → String
            builder.HasIndex(u => u.WalletAddress).IsUnique(); // Кошельки уникальны
            builder.HasIndex(u => u.GitHubId).IsUnique(); // GitHub привязан к 1 юзеру
        }
    }
    */
}
