using Microsoft.EntityFrameworkCore;
using SocialTDD.Domain.Models;

namespace SocialTDD.Data.Contexts;

public class SocialDbContext : DbContext
{
    public SocialDbContext(DbContextOptions<SocialDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<DirectMessage> DirectMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.HasIndex(e => e.Username)
                .IsUnique();
            entity.HasIndex(e => e.Email)
                .IsUnique();
        });

        // DirectMessage configuration
        modelBuilder.Entity<DirectMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(1000);
            entity.Property(e => e.SentAt)
                .IsRequired();

            // Foreign keys
            entity.HasOne(dm => dm.Sender)
                .WithMany()
                .HasForeignKey(dm => dm.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(dm => dm.Recipient)
                .WithMany()
                .HasForeignKey(dm => dm.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for performance
            entity.HasIndex(e => e.RecipientId);
            entity.HasIndex(e => e.SenderId);
            entity.HasIndex(e => new { e.SenderId, e.RecipientId });
        });
    }
}


