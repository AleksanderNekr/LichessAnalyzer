using Backend.DataManagement.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.DataManagement.Users;

public class UsersContext(DbContextOptions<UsersContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }

    public DbSet<User> Users { get; set; } = default!;

    public DbSet<AnalyticsList> AnalyticsLists { get; set; } = default!;

    public DbSet<Player> Players { get; set; } = default!;

    public DbSet<Team> Teams { get; set; } = default!;
}
