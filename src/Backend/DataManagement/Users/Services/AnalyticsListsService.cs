using Backend.DataManagement.LichessApi;
using Backend.DataManagement.LichessApi.ServiceResponsesModels;
using Backend.DataManagement.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.DataManagement.Users.Services;

public class AnalyticsListsService(
    UsersContext                   context,
    CachedDataService              cachedDataService,
    ILogger<AnalyticsListsService> logger)
{
    public async Task<(AnalyticsList?, string)> CreateByPlayersAsync(
        Guid                listId,
        string              listName,
        User                creator,
        ICollection<string> playersIds,
        CancellationToken   cancellationToken = default)
    {
        if (await ListsLimitReachedAsync())
        {
            logger.LogDebug("Max lists limit reached for user: {Name}", creator.Name);

            return (null, $"Max lists limit: {creator.MaxListsCount} reached! Unable to create");
        }

        if (await ListNameTakenAsync())
        {
            logger.LogDebug("List with Name {ListName} exists for user {Id}:{Name}",
                            listName,
                            creator.Id,
                            creator.Name);

            return (null, $"List with Name {listName} already exists in your collection");
        }

        AnalyticsList list = new(listId, listName, creator.Id);
        if (playersIds.Count > 0)
        {
            IEnumerable<PlayerResponse> cachedPlayers = await cachedDataService.GetChessPlayersAsync(
                                                            playersIds.ToList(),
                                                            [ ],
                                                            [ ],
                                                            cancellationToken);
            list.ListedPlayers = cachedPlayers.Select(player => new Player(player.Id, listId))
                                              .ToList();
        }

        await context.AnalyticsLists.AddAsync(list, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return (list, string.Empty);

        async Task<bool> ListsLimitReachedAsync()
        {
            return creator.AnalyticsLists.Count == creator.MaxListsCount
                || await context.AnalyticsLists.CountAsync(
                       l => l.CreatorId == creator.Id,
                       cancellationToken)
                >= creator.MaxListsCount;
        }

        async Task<bool> ListNameTakenAsync()
        {
            return await context.AnalyticsLists.SingleOrDefaultAsync(
                           l => l.Name == listName,
                           cancellationToken)
                       is not null;
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


        IEnumerable<PlayerResponse> cachedPlayers = await cachedDataService.GetChessPlayersAsync(
                                                        playersIds.ToList(),
                                                        [ ],
                                                        [ ],
                                                        cancellationToken);
        list.ListedPlayers = list.ListedPlayers.Concat(cachedPlayers
                                                      .Take(RemainCapacity())
                                                      .Select(player => new Player(player.Id, list.Id)))
                                 .ToList();
        await context.SaveChangesAsync(cancellationToken);

        return list;

        bool EmptyOrWrongCreator() => playersIds.Length == 0 || creator.Id != list.CreatorId;

        int RemainCapacity() => creator.MaxPlayersInList - list.ListedPlayers.Count();
    }

    public async Task<AnalyticsList> DeletePlayersAsync(
        AnalyticsList       list,
        ICollection<string> playersIds,
        CancellationToken   cancellationToken = default)
    {
        if (playersIds.Count > 0)
        {
            return list;
        }

        list.ListedPlayers = list.ListedPlayers.Where(player => !playersIds.Contains(player.Id))
                                 .ToList();
        await context.SaveChangesAsync(cancellationToken);

        return list;
    }
}
