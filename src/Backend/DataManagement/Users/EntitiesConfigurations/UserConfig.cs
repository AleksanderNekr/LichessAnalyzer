using Backend.DataManagement.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.DataManagement.Users.EntitiesConfigurations;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(user => user.Id)
               .HasColumnName("id");

        builder.Property(user => user.Name)
               .HasColumnName("name")
               .HasColumnType("VARCHAR(255)");

        builder.Property(user => user.MaxPlayersInList)
               .HasColumnName("max_players_in_list")
               .HasDefaultValue(100);

        builder.Property(user => user.MaxListsCount)
               .HasColumnName("max_lists_count")
               .HasDefaultValue(30);

        builder.HasKey(user => user.Id)
               .HasName("pk_users");

        builder.HasIndex(user => user.Name)
               .IsUnique()
               .HasDatabaseName("uq_users_names");

        builder.HasMany(user => user.AnalyticsLists)
               .WithOne(lists => lists.Creator)
               .HasForeignKey(list => list.CreatorId)
               .HasConstraintName("fk_player_lists_by_user")
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(user => user.OrganisedTeams)
               .WithOne(team => team.Organiser)
               .HasForeignKey(team => team.OrganiserId)
               .HasConstraintName("fk_organises_by_user")
               .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("users",
                        static tableBuilder =>
                        {
                            tableBuilder.HasCheckConstraint(
                                "ch_max_players_val_range",
                                "max_players_in_list >= 5 AND max_players_in_list <= 500");
                            tableBuilder.HasCheckConstraint(
                                "ch_max_lists_val_range",
                                "max_lists_count >= 5 AND max_lists_count <= 100");
                        });
    }
}
