using Backend.DataManagement.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.DataManagement.Users.EntitiesConfigurations;

public class TeamConfig : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.Property(team => team.Id)
               .HasColumnName("club_id");

        builder.Property(team => team.OrganiserId)
               .HasColumnName("user_id");

        builder.HasKey(team => team.Id)
               .HasName("pk_organises");

        builder.ToTable("organises");
    }
}
