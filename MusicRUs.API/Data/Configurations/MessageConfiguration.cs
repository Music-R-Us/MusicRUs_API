using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicRUs.API.Models.Entities;

namespace MusicRUs.API.Data.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("messages", t =>
        {
            t.HasCheckConstraint("ck_messages_not_empty", "char_length(btrim(content)) > 0");
            t.HasCheckConstraint("ck_messages_not_self", "sender_id <> recipient_id");
        });

        // Samtale mellem to brugere og indbakke, begge i tidsorden
        builder.HasIndex(m => new { m.SenderId, m.RecipientId, m.CreatedAt });
        builder.HasIndex(m => new { m.RecipientId, m.CreatedAt });

        builder.HasOne(m => m.Sender).WithMany().HasForeignKey(m => m.SenderId)
               .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(m => m.Recipient).WithMany().HasForeignKey(m => m.RecipientId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}