using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoApp.Entities;

public class PostEntityTypeConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.Title).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Body).IsRequired().HasMaxLength(500);
        builder.Property(b => b.CreatedAt).IsRequired();
        builder.Property(b => b.UpdatedAt).IsRequired(false).IsConcurrencyToken();

        builder
            .HasMany(p => p.Users)
            .WithMany(u => u.Posts)
            .UsingEntity<UserPost>(
                j =>
                    j.HasOne(up => up.User)
                        .WithMany(u => u.UserPosts)
                        .HasForeignKey(up => up.UserId),
                j =>
                    j.HasOne(up => up.Post)
                        .WithMany(p => p.UserPosts)
                        .HasForeignKey(up => up.PostId),
                j =>
                {
                    j.HasKey(
                        t =>
                            new
                            {
                                t.UserId,
                                t.PostId,
                                t.Relation
                            }
                    );
                    j.Property(t => t.Relation).IsRequired();
                }
            );
    }
}
