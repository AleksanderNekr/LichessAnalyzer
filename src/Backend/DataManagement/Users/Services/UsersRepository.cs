using Backend.DataManagement.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.DataManagement.Users.Services;

public class UsersRepository(UsersContext context, ILogger<UsersRepository> logger)
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
            user.AnalyticsLists = context.AnalyticsLists.Where(list => list.CreatorId == id).ToList();
        }

        if (includeTeams)
        {
            user.OrganisedTeams = context.Teams.Where(team => team.OrganiserId == id).ToList();
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
            user.AnalyticsLists = context.AnalyticsLists.Where(list => list.CreatorId == user.Id).ToList();
        }

        if (includeTeams)
        {
            user.OrganisedTeams = context.Teams.Where(team => team.OrganiserId == user.Id).ToList();
        }

        return user;
    }

    public async Task<User?> TryCreateUserAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        User? found = await context.Users.SingleOrDefaultAsync(u => u.Name == name, cancellationToken);
        if (found is not null)
        {
            logger.LogDebug("User with name: {Name} already registered", name);
            return null;
        }
        
        User user = new(id, name);

        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<bool> TryDeleteUserAsync(Guid id, CancellationToken cancellationToken)
    {
        User? user = await context.Users.FindAsync([ id ], cancellationToken);
        if (user is null)
        {
            logger.LogDebug("User with id: {ID} can't be found for delete", id);
            return false;
        }
        
        context.Users.Remove(user);

        int rowsDeleted = await context.SaveChangesAsync(cancellationToken);

        return rowsDeleted == 1;
    }
}
