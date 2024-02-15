using Backend.DataManagement.Users.Entities;

namespace Backend.DataManagement.Users.Services;

public class AnalyticsListsService(UsersContext context)
{
    public async Task<AnalyticsList> CreateByPlayersAsync(
        Guid              listId,
        string            listName,
        User              creator,
        CancellationToken cancellationToken = default,
        params string[]   playersIds)
    {
        AnalyticsList list = new(listId, listName, creator.Id);
        if (playersIds.Length > 0)
        {
            list.ListedPlayers = playersIds.Select(playerId => new Player(playerId, listId));
        }

        await context.AnalyticsLists.AddAsync(list, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return list;
    }

    public async Task<bool> UpdateListNameAsync(
        User              owner,
        Guid              listId,
        string            newListName,
        CancellationToken cancellationToken = default)
    {
        AnalyticsList? list = await context.AnalyticsLists.FindAsync([ listId ], cancellationToken);
        if (list is null || list.CreatorId != owner.Id)
        {
            return false;
        }

        list.Name = newListName;
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteListAsync(
        User              owner,
        Guid              listId,
        CancellationToken cancellationToken = default)
    {
        AnalyticsList? list = await context.AnalyticsLists.FindAsync([ listId ], cancellationToken);
        if (list is null || list.CreatorId != owner.Id)
        {
            return false;
        }

        context.AnalyticsLists.Remove(list);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<AnalyticsList> AddPlayersAsync(
        AnalyticsList     list,
        CancellationToken cancellationToken = default,
        params string[]   playersIds)
    {
        if (playersIds.Length == 0)
        {
            return list;
        }

        list.ListedPlayers = list.ListedPlayers.Concat(playersIds.Select(playerId => new Player(playerId, list.Id)));
        await context.SaveChangesAsync(cancellationToken);

        return list;
    }

    public async Task<AnalyticsList> DeletePlayersAsync(
        AnalyticsList     list,
        CancellationToken cancellationToken = default,
        params string[]   playersIds)
    {
        if (playersIds.Length == 0)
        {
            return list;
        }

        list.ListedPlayers = list.ListedPlayers.Where(player => !playersIds.Contains(player.Id));
        await context.SaveChangesAsync(cancellationToken);

        return list;
    }
}
