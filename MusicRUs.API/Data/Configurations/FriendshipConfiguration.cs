using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.ToTable("friendships", t => t.HasCheckConstraint(
            "ck_friendships_not_self", "requester_id <> addressee_id"));

        builder.Property(f => f.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(f => f.Requester).WithMany().HasForeignKey(f => f.RequesterId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(f => f.Addressee).WithMany().HasForeignKey(f => f.AddresseeId)
               .OnDelete(DeleteBehavior.Cascade);

        // Unique pair no matter the direction: ux_friendships_pair is created with raw SQL in the AddSocial migration
    }
}