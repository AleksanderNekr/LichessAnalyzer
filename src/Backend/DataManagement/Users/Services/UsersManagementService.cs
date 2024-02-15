using Backend.DataManagement.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.DataManagement.Users.Services;

public class UsersManagementService(UsersContext context)
{
    public async ValueTask<User?> FindByIdAsync(
        Guid              id,
        bool              includeAnalyticsLists = false,
        bool              includeTeams          = false,
        CancellationToken cancellationToken     = default)
    {
        User? user = await context.Users.FindAsync([ id ], cancellationToken);
        if (user is null)
        {
            return user;
        }

        if (includeAnalyticsLists)
        {
            user.AnalyticsLists = context.AnalyticsLists.Where(list => list.CreatorId == id)
                                         .AsEnumerable();
        }

        if (includeTeams)
        {
            user.OrganisedTeams = context.Teams.Where(team => team.OrganiserId == id)
                                         .AsEnumerable();
        }

        return user;
    }

    public async ValueTask<User?> FindByNameAsync(
        string            name,
        bool              includeAnalyticsLists = false,
        bool              includeTeams          = false,
        CancellationToken cancellationToken     = default)
    {
        User? user = await context.Users.SingleOrDefaultAsync(u => u.Name == name, cancellationToken);
        if (user is null)
        {
            return user;
        }

        if (includeAnalyticsLists)
        {
            user.AnalyticsLists = context.AnalyticsLists.Where(list => list.CreatorId == user.Id)
                                         .AsEnumerable();
        }

        if (includeTeams)
        {
            user.OrganisedTeams = context.Teams.Where(team => team.OrganiserId == user.Id)
                                         .AsEnumerable();
        }

        return user;
    }

    public async Task<User> CreateUserAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        User user = new(id, name);

        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        User user = await context.Users.FindAsync([ id ], cancellationToken)
                 ?? throw new InvalidOperationException($"User with ID: {id} can't be found");
        context.Users.Remove(user);

        int rowsDeleted = await context.SaveChangesAsync(cancellationToken);

        return rowsDeleted == 1;
    }
}
