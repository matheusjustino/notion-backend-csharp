namespace NotionBackend.Api.Infrastructure.Persistence.Mapping;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotionBackend.Api.Domain.Entities;

public class DocumentMap : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> modelBuilder)
    {
        modelBuilder
            .HasIndex(d => d.Id).IsUnique();

        modelBuilder
            .Property(d => d.Title)
            .IsRequired();

        modelBuilder
            .Property(d => d.AuthorId)
            .IsRequired();

        modelBuilder
            .Property(d => d.IsPublished)
            .HasDefaultValue(false);

        modelBuilder
            .Property(d => d.IsArchived)
            .HasDefaultValue(false);

        modelBuilder
            .HasOne(d => d.ParentDocument)
            .WithMany()
            .HasForeignKey(d => d.ParentDocumentId);

        modelBuilder
            .HasOne(d => d.ParentDocument)
            .WithMany()
            .HasForeignKey(d => d.ParentDocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .HasOne(d => d.Author)
            .WithMany(u => u.Documents)
            .HasForeignKey(d => d.AuthorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}