namespace NotionBackend.Api.Infrastructure.Persistence;

using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using NotionBackend.Api.Domain.Entities;
using NotionBackend.Api.Infrastructure.Persistence.Mapping;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Document> Documents { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    [ModuleInitializer]
    public static void Initialize()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(new UserMap().Configure);
        modelBuilder.Entity<Document>(new DocumentMap().Configure);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>().ToList();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries<BaseEntity>().ToList();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.Now;
                entry.Entity.UpdatedAt = DateTime.Now;
            }

            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.Now;
            }
        }

        return base.SaveChanges();
    }
}