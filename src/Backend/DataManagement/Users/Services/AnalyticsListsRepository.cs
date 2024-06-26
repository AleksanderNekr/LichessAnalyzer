﻿using Backend.DataManagement.LichessApi;
using Backend.DataManagement.LichessApi.ServiceResponsesModels;
using Backend.DataManagement.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.DataManagement.Users.Services;

public class AnalyticsListsRepository(
    UsersContext                      context,
    CachedDataService                 cachedDataService,
    ILogger<AnalyticsListsRepository> logger)
{
    public async ValueTask<AnalyticsList?> GetByIdAsync(User owner, Guid id, CancellationToken cancellationToken)
    {
        AnalyticsList? list = owner.AnalyticsLists?.SingleOrDefault(list => list.Id == id)
                           ?? await context.AnalyticsLists.FindAsync([ id ], cancellationToken);

        if (list is null || list.CreatorId != owner.Id)
        {
            return null;
        }

        return list;
    }

    public async ValueTask<AnalyticsList?> GetByIdWithPlayersAsync(User owner, Guid id, CancellationToken cancellationToken)
    {
        AnalyticsList? list = owner.AnalyticsLists?.AsQueryable()
                                   .Include(list => list.ListedPlayers)
                                   .AsNoTrackingWithIdentityResolution()
                                   .SingleOrDefault(list => list.Id == id)
                           ?? await context.AnalyticsLists.Include(list => list.ListedPlayers)
                                           .AsNoTrackingWithIdentityResolution()
                                           .SingleOrDefaultAsync(list => list.Id == id, cancellationToken);

        if (list is null || list.CreatorId != owner.Id)
        {
            return null;
        }

        return list;
    }

    public IQueryable<AnalyticsList> GetAll(User owner)
    {
        return owner.AnalyticsLists?.AsQueryable()
                    .AsNoTrackingWithIdentityResolution()
            ?? context.AnalyticsLists.Where(list => list.CreatorId == owner.Id)
                      .AsNoTrackingWithIdentityResolution();
    }

    public IQueryable<AnalyticsList> GetAllWithPlayers(User owner)
    {
        return owner.AnalyticsLists?
                    .AsQueryable()
                    .Include(list => list.ListedPlayers)
                    .AsNoTrackingWithIdentityResolution()
            ?? context.AnalyticsLists
                      .Include(list => list.ListedPlayers)
                      .Where(list => list.CreatorId == owner.Id)
                      .AsNoTrackingWithIdentityResolution();
    }

    public async Task<(AnalyticsList?, string)> CreateByPlayersAsync(
        Guid                listId,
        string              listName,
        User                creator,
        ICollection<string> playersIds,
        CancellationToken   cancellationToken = default)
    {
        if (await ListsLimitReachedAsync(creator, cancellationToken))
        {
            logger.LogDebug("Max lists limit reached for user: {Name}", creator.Name);

            return (null, $"Max lists limit: {creator.MaxListsCount} reached! Unable to create");
        }

        if (await ListNameTakenAsync(listName, cancellationToken))
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
    }

    public async Task<(AnalyticsList?, string)> CreateByTeamsAsync(
        Guid                listId,
        string              listName,
        User                creator,
        ICollection<string> teamsIds,
        CancellationToken   cancellationToken = default)
    {
        if (await ListsLimitReachedAsync(creator, cancellationToken))
        {
            logger.LogDebug("Max lists limit reached for user: {Name}", creator.Name);

            return (null, $"Max lists limit: {creator.MaxListsCount} reached! Unable to create");
        }

        if (await ListNameTakenAsync(listName, cancellationToken))
        {
            logger.LogDebug("List with Name {ListName} exists for user {Id}:{Name}",
                            listName,
                            creator.Id,
                            creator.Name);

            return (null, $"List with Name {listName} already exists in your collection");
        }

        AnalyticsList list = new(listId, listName, creator.Id);
        if (teamsIds.Count > 0)
        {
            IEnumerable<TeamResponse> cachedTeams = await cachedDataService.GetTeamsAsync(
                                                        teamsIds.ToList(),
                                                        withParticipants: true,
                                                        cancellationToken: cancellationToken);
            list.ListedPlayers = cachedTeams.SelectMany(team => team.Participants
                                                                    .Select(playerId =>
                                                                                new Player(playerId, listId)))
                                            .ToList();
        }

        await context.AnalyticsLists.AddAsync(list, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return (list, string.Empty);
    }

    private async Task<bool> ListNameTakenAsync(string listName, CancellationToken cancellationToken)
    {
        return await context.AnalyticsLists.SingleOrDefaultAsync(
                       l => l.Name == listName,
                       cancellationToken)
                   is not null;
    }

    private async Task<bool> ListsLimitReachedAsync(User creator, CancellationToken cancellationToken)
    {
        return creator.AnalyticsLists is null
            && await CountCreatorsListsAsync() >= creator.MaxListsCount
            || creator.AnalyticsLists?.Count >= creator.MaxListsCount;

        Task<int> CountCreatorsListsAsync()
        {
            return context.AnalyticsLists.CountAsync(l => l.CreatorId == creator.Id,
                                                     cancellationToken);
        }
    }

    public async Task<(AnalyticsList?, ListManipulationResult)> UpdateListNameAsync(
        User              owner,
        Guid              listId,
        string            newListName,
        CancellationToken cancellationToken = default)
    {
        AnalyticsList? list = await context.AnalyticsLists.FindAsync([ listId ], cancellationToken);
        if (EmptyOrWrongCreator())
        {
            return (null, ListManipulationResult.ListNotFound);
        }

        list!.Name = newListName;
        await context.SaveChangesAsync(cancellationToken);

        return (list, ListManipulationResult.Success);

        bool EmptyOrWrongCreator() => list is null || list.CreatorId != owner.Id;
    }

    public async Task<ListManipulationResult> DeleteListAsync(
        User              owner,
        Guid              listId,
        CancellationToken cancellationToken = default)
    {
        AnalyticsList? list = await context.AnalyticsLists.FindAsync([ listId ], cancellationToken);
        if (list is null || list.CreatorId != owner.Id)
        {
            return ListManipulationResult.ListNotFound;
        }

        context.AnalyticsLists.Remove(list);
        await context.SaveChangesAsync(cancellationToken);

        return ListManipulationResult.Success;
    }

    public async Task<(AnalyticsList?, ListManipulationResult)> AddPlayersAsync(
        User              creator,
        Guid              listId,
        List<string>      playersIds,
        CancellationToken cancellationToken = default)
    {
        AnalyticsList? list = await GetByIdWithPlayersAsync(creator, listId, cancellationToken);
        if (list is null)
        {
            return (null, ListManipulationResult.ListNotFound);
        }

        playersIds = playersIds.Where(id => list.ListedPlayers!.All(
                                          p => !p.Id.Equals(id,
                                                            StringComparison.OrdinalIgnoreCase)))
                               .ToList();
        if (playersIds.Count == 0)
        {
            return (list, ListManipulationResult.Success);
        }

        IEnumerable<PlayerResponse> playersToAdd = await cachedDataService.GetChessPlayersAsync(
                                                       playersIds,
                                                       [ ],
                                                       [ ],
                                                       cancellationToken);

        await context.Players.AddRangeAsync(playersToAdd
                                           .Take(RemainCapacity())
                                           .Select(p => new Player(p.Id, list.Id)),
                                            cancellationToken);

        context.AnalyticsLists.Entry(list).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);

        return (list, ListManipulationResult.Success);

        int RemainCapacity() => creator.MaxPlayersInList - list.ListedPlayers!.Count;
    }

    public async Task<(AnalyticsList?, ListManipulationResult)> DeletePlayersAsync(
        User              owner,
        Guid              listId,
        List<string>      playersIds,
        CancellationToken cancellationToken = default)
    {
        AnalyticsList? list = await GetByIdWithPlayersAsync(owner, listId, cancellationToken);
        if (list is null)
        {
            return (null, ListManipulationResult.ListNotFound);
        }

        IEnumerable<Player> playersToDelete = list.ListedPlayers!.Where(
                                                       player => playersIds.Contains(
                                                           player.Id,
                                                           StringComparer.OrdinalIgnoreCase))
                                                  .ToList();
        foreach (Player player in playersToDelete)
        {
            context.Players.Entry(player).State = EntityState.Deleted;
        }

        logger.LogDebug("Deleted {Count} players from list {Name}",
                        playersToDelete.Count(),
                        list.Name);

        context.AnalyticsLists.Entry(list).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);

        return (list, ListManipulationResult.Success);
    }
}
