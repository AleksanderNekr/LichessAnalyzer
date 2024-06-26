﻿using Backend.Api.RequestModels;
using Backend.Auth;
using Backend.DataManagement.LichessApi;
using Backend.DataManagement.LichessApi.ServiceResponsesModels;
using Backend.DataManagement.Users.Entities;
using Backend.DataManagement.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api;

[ApiController]
[Route("api")]
public class AnalyticListsController(
    AuthService                      authService,
    AnalyticsListsRepository         listsRepository,
    ILogger<AnalyticListsController> logger) : Controller
{
    private readonly Lazy<List<PlayerStat>> _allStats = new(Enum.GetValues<PlayerStat>().ToList());

    private readonly Lazy<List<PlayCategory>> _allCategories = new(Enum.GetValues<PlayCategory>().ToList());

    [HttpGet("lists/{id:guid}")]
    [Authorize(AuthExtensions.LichessAuthPolicyName)]
    public async Task<ActionResult<AnalyticsList>> GetById(
        [FromRoute] Guid  id,
        [FromQuery] bool  includePlayers    = true,
        CancellationToken cancellationToken = default)
    {
        User? owner = await authService.GetCurrentUserAsync(HttpContext.User, cancellationToken);
        if (owner is null)
        {
            logger.LogWarning("Current user is null");

            return Forbid(AuthExtensions.AuthenticationScheme);
        }

        AnalyticsList? list = includePlayers
                                  ? await listsRepository.GetByIdWithPlayersAsync(owner, id, cancellationToken)
                                  : await listsRepository.GetByIdAsync(owner, id, cancellationToken);
        if (list is null)
        {
            logger.LogWarning("Tried to get analytics list with ID: {Id}, but not found", id);

            return NotFound($"Analytics list with ID: {id} not found");
        }

        Guid? userId = await GetCurrentUserIdAsync(cancellationToken);

        if (list.CreatorId == userId)
        {
            return Ok(list);
        }

        logger.LogWarning("Tried to get list ({Id}) not owned by User ({UserId})", id, userId);

        return Forbid(AuthExtensions.AuthenticationScheme);
    }

    [HttpGet("lists")]
    [Authorize(AuthExtensions.LichessAuthPolicyName)]
    public async Task<ActionResult<IEnumerable<AnalyticsList>>> GetAll(
        [FromQuery] bool  includePlayers    = true,
        CancellationToken cancellationToken = default)
    {
        User? owner = await authService.GetCurrentUserAsync(HttpContext.User, cancellationToken);
        if (owner is null)
        {
            logger.LogWarning("Current user is null");

            return Forbid(AuthExtensions.AuthenticationScheme);
        }

        IEnumerable<AnalyticsList> lists = includePlayers
                                               ? listsRepository.GetAllWithPlayers(owner)
                                                                .AsEnumerable()
                                               : listsRepository.GetAll(owner)
                                                                .AsEnumerable();

        return Ok(lists);
    }

    [HttpPost("players-lists")]
    [Authorize(AuthExtensions.LichessAuthPolicyName)]
    public async Task<ActionResult<AnalyticsList>> CreateByPlayers(
        [FromBody] CreateListRequestBodyModel requestBody,
        CancellationToken                     cancellationToken)
    {
        User? creator = await authService.GetCurrentUserAsync(HttpContext.User, cancellationToken);
        if (creator is null)
        {
            return Forbid(AuthExtensions.AuthenticationScheme);
        }

        (AnalyticsList? list, string message) = await listsRepository.CreateByPlayersAsync(
                                                    Guid.NewGuid(),
                                                    requestBody.Name,
                                                    creator,
                                                    requestBody.Ids,
                                                    cancellationToken);
        if (list is null)
        {
            return Conflict(message);
        }

        return Created("/api/lists/" + list.Id, list);
    }

    [HttpPost("teams-lists")]
    [Authorize(AuthExtensions.LichessAuthPolicyName)]
    public async Task<ActionResult<AnalyticsList>> CreateByTeams(
        [FromBody] CreateListRequestBodyModel requestBody,
        CancellationToken                     cancellationToken)
    {
        User? creator = await authService.GetCurrentUserAsync(HttpContext.User, cancellationToken);
        if (creator is null)
        {
            return Forbid(AuthExtensions.AuthenticationScheme);
        }

        (AnalyticsList? list, string message) = await listsRepository.CreateByTeamsAsync(
                                                    Guid.NewGuid(),
                                                    requestBody.Name,
                                                    creator,
                                                    requestBody.Ids,
                                                    cancellationToken);
        if (list is null)
        {
            return Conflict(message);
        }

        return Created("/api/lists/" + list.Id, list);
    }

    [HttpPut("lists/{id:guid}/{newName}")]
    [Authorize(AuthExtensions.LichessAuthPolicyName)]
    public async Task<ActionResult<AnalyticsList>> UpdateListName(
        [FromRoute] Guid   id,
        [FromRoute] string newName,
        CancellationToken  cancellationToken)
    {
        User? creator = await authService.GetCurrentUserAsync(HttpContext.User, cancellationToken);
        if (creator is null)
        {
            return Forbid(AuthExtensions.AuthenticationScheme);
        }

        (AnalyticsList? newList, ListManipulationResult updateResult) =
            await listsRepository.UpdateListNameAsync(creator, id, newName, cancellationToken);
        if (updateResult == ListManipulationResult.ListNotFound)
        {
            return NotFound($"List with id {id} not found for user {creator.Name}");
        }

        return Ok(newList!);
    }

    [HttpDelete("lists/{id:guid}")]
    [Authorize(AuthExtensions.LichessAuthPolicyName)]
    public async Task<IActionResult> DeleteList(
        [FromRoute] Guid  id,
        CancellationToken cancellationToken)
    {
        User? creator = await authService.GetCurrentUserAsync(HttpContext.User, cancellationToken);
        if (creator is null)
        {
            return Forbid(AuthExtensions.AuthenticationScheme);
        }

        ListManipulationResult result = await listsRepository.DeleteListAsync(creator,
                                                                              id,
                                                                              cancellationToken);
        if (result == ListManipulationResult.ListNotFound)
        {
            return NotFound($"List with id {id} not found for user {creator.Name}");
        }

        return NoContent();
    }

    [HttpPost("lists/{id:guid}/players")]
    [Authorize(AuthExtensions.LichessAuthPolicyName)]
    public async Task<ActionResult<AnalyticsList>> AddPlayers(
        [FromRoute] Guid         id,
        [FromBody]  List<string> playersIds,
        CancellationToken        cancellationToken)
    {
        User? creator = await authService.GetCurrentUserAsync(HttpContext.User, cancellationToken);
        if (creator is null)
        {
            return Forbid(AuthExtensions.AuthenticationScheme);
        }

        (AnalyticsList? newList, ListManipulationResult result) =
            await listsRepository.AddPlayersAsync(creator,
                                                  id,
                                                  playersIds,
                                                  cancellationToken);
        if (result == ListManipulationResult.ListNotFound)
        {
            return NotFound($"List with id {id} not found for user {creator.Name}");
        }

        return Ok(newList!);
    }

    [HttpDelete("lists/{id:guid}/players")]
    [Authorize(AuthExtensions.LichessAuthPolicyName)]
    public async Task<ActionResult<AnalyticsList>> DeletePlayers(
        [FromRoute] Guid         id,
        [FromBody]  List<string> playersIds,
        CancellationToken        cancellationToken)
    {
        User? creator = await authService.GetCurrentUserAsync(HttpContext.User, cancellationToken);
        if (creator is null)
        {
            return Forbid(AuthExtensions.AuthenticationScheme);
        }

        (AnalyticsList? newList, ListManipulationResult result) =
            await listsRepository.DeletePlayersAsync(creator,
                                                     id,
                                                     playersIds,
                                                     cancellationToken);
        if (result == ListManipulationResult.ListNotFound)
        {
            return NotFound($"List with id {id} not found for user {creator.Name}");
        }

        return Ok(newList!);
    }

    [HttpGet("lists/{id:guid}/data/{type}")]
    [Authorize(AuthExtensions.LichessAuthPolicyName)]
    public async Task<IActionResult> ExportPlayersInList(
        [FromRoute]    Guid              id,
        [FromRoute]    ExportFileType    type,
        [FromServices] CachedDataService cachedDataService,
        CancellationToken                cancellationToken)
    {
        User? owner = await authService.GetCurrentUserAsync(HttpContext.User, cancellationToken);
        if (owner is null)
        {
            logger.LogWarning("Current user is null");

            return Forbid(AuthExtensions.AuthenticationScheme);
        }

        AnalyticsList? list = await listsRepository.GetByIdWithPlayersAsync(owner, id, cancellationToken);
        if (list is null)
        {
            logger.LogWarning("Tried to get analytics list with ID: {Id}, but not found", id);

            return NotFound($"Analytics list with ID: {id} not found");
        }

        IEnumerable<PlayerResponse> playersToExport = await cachedDataService.GetChessPlayersAsync(
                                                          list.ListedPlayers!.Select(player => player.Id).ToList(),
                                                          _allStats.Value,
                                                          _allCategories.Value,
                                                          cancellationToken);

        return type switch
        {
            ExportFileType.Excel => ExportMethods.ToExcel(playersToExport),
            ExportFileType.Csv   => ExportMethods.ToCsv(playersToExport),
            _                    => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    private async Task<Guid?> GetCurrentUserIdAsync(CancellationToken cancellationToken)
    {
        return (await authService.GetCurrentUserAsync(HttpContext.User, cancellationToken))?.Id;
    }
}
