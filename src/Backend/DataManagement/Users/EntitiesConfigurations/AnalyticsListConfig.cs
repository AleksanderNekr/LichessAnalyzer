using Backend.DataManagement.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.DataManagement.Users.EntitiesConfigurations;

public class AnalyticsListConfig : IEntityTypeConfiguration<AnalyticsList>
{
    public void Configure(EntityTypeBuilder<AnalyticsList> builder)
    {
        builder.Property(list => list.Id)
               .HasColumnName("list_id");

        builder.Property(list => list.Name)
               .HasColumnName("name")
               .HasColumnType("VARCHAR(255)");

        builder.Property(list => list.CreatorId)
               .HasColumnName("user_id");
               
        builder.HasKey(list => list.Id)
               .HasName("pk_players_lists");
        
        builder.HasIndex(list => new { list.Name, list.CreatorId })
               .IsUnique()
               .HasDatabaseName("uq_names_per_user");

        builder.HasMany(list => list.ListedPlayers)
               .WithOne(player => player.ContainingList)
               .HasForeignKey(player => player.ContainingListId)
               .HasConstraintName("fk_contains_by_lists")
               .OnDelete(DeleteBehavior.Cascade);
                
        builder.ToTable("players_lists",
                        tableBuilder =>
                        {
                            tableBuilder.HasCheckConstraint(
                                "ch_name_len_range",
                                "LENGTH(name) >= 1");
                        });
    }
}
