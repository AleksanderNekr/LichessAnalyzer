using Backend.DataManagement.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.DataManagement.Users.EntitiesConfigurations;

public class PlayerConfig : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.Property(player => player.Id)
               .HasColumnName("player_id");

        builder.Property(player => player.ContainingListId)
               .HasColumnName("list_id");

        builder.HasKey(player => new { player.ContainingListId, player.Id })
               .HasName("pk_contains");

        builder.ToTable("contains");
    }
}
