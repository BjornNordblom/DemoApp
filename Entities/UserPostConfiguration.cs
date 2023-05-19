using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoApp.Entities;

public class UserPostEntityTypeConfiguration : IEntityTypeConfiguration<UserPost>
{
    public void Configure(EntityTypeBuilder<UserPost> builder)
    {
        builder.HasKey(
            up =>
                new
                {
                    up.UserId,
                    up.PostId,
                    up.Relation
                }
        );
        builder.Property(up => up.Relation).IsRequired();
        builder
            .HasOne(up => up.User)
            .WithMany(u => u.UserAuthoredPosts)
            .HasForeignKey(up => up.UserId);
        builder
            .HasOne(up => up.Post)
            .WithMany(p => p.UserWatchedPosts)
            .HasForeignKey(up => up.PostId);
    }
}
