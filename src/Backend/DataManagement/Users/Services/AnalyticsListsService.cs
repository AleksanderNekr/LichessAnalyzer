using Backend.DataManagement.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.DataManagement.Users.Services;

public class AnalyticsListsService(UsersContext context, ILogger<AnalyticsListsService> logger)
{
    public async Task<AnalyticsList?> CreateByPlayersAsync(
        Guid                listId,
        string              listName,
        User                creator,
        ICollection<string> playersIds,
        CancellationToken   cancellationToken = default)
    {
        if (await ListsLimitReachedAsync())
        {
            logger.LogDebug("Max lists limit reached for user: {Name}", creator.Name);

            return null;
        }

        AnalyticsList list = new(listId, listName, creator.Id);
        if (playersIds.Count > 0)
        {
            list.ListedPlayers = playersIds.Select(playerId => new Player(playerId, listId));
        }

        await context.AnalyticsLists.AddAsync(list, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return list;

        async Task<bool> ListsLimitReachedAsync()
        {
            return creator.AnalyticsLists.Count == creator.MaxListsCount
                || await context.AnalyticsLists.CountAsync(
                       l => l.CreatorId == creator.Id,
                       cancellationToken)
                >= creator.MaxListsCount;
        }
    }

    public async Task<bool> UpdateListNameAsync(
        User              owner,
        Guid              listId,
        string            newListName,
        CancellationToken cancellationToken = default)
    {
        AnalyticsList? list = await context.AnalyticsLists.FindAsync([ listId ], cancellationToken);
        if (EmptyOrWrongCreator())
        {
            return false;
        }

        list!.Name = newListName;
        await context.SaveChangesAsync(cancellationToken);

        return true;

        bool EmptyOrWrongCreator() => list is null || list.CreatorId != owner.Id;
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
        User              creator,
        AnalyticsList     list,
        CancellationToken cancellationToken = default,
        params string[]   playersIds)
    {
        if (EmptyOrWrongCreator())
        {
            return list;
        }

        list.ListedPlayers = list.ListedPlayers.Concat(playersIds
                                                      .Take(RemainCapacity())
                                                      .Select(playerId => new Player(playerId, list.Id)));
        await context.SaveChangesAsync(cancellationToken);

        return list;

        bool EmptyOrWrongCreator() => playersIds.Length == 0 || creator.Id != list.CreatorId;

        int RemainCapacity() => creator.MaxPlayersInList - list.ListedPlayers.Count();
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
