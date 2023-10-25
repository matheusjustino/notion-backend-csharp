namespace NotionBackend.Api.Infrastructure.Persistence.Mapping;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotionBackend.Api.Domain.Entities;

public class UserMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> modelBuilder)
    {
        modelBuilder
            .HasIndex(u => u.Id).IsUnique();

        modelBuilder
            .HasIndex(p => p.Email).IsUnique();
    }
}