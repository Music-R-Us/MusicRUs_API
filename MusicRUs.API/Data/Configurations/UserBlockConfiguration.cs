using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class UserBlockConfiguration : IEntityTypeConfiguration<UserBlock>
{
    public void Configure(EntityTypeBuilder<UserBlock> builder)
    {
        builder.ToTable("user_blocks", t => t.HasCheckConstraint(
            "ck_user_blocks_not_self", "blocker_id <> blocked_id"));
        builder.HasKey(b => new { b.BlockerId, b.BlockedId });

        builder.HasOne(b => b.Blocker).WithMany().HasForeignKey(b => b.BlockerId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(b => b.Blocked).WithMany().HasForeignKey(b => b.BlockedId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}