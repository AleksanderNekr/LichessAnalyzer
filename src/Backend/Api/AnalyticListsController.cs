using Backend.Api.RequestModels;
using Backend.Auth;
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

        Guid userId = await GetCurrentUserIdAsync(cancellationToken);

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

    private async Task<Guid> GetCurrentUserIdAsync(CancellationToken cancellationToken)
    {
        return (await authService.GetCurrentUserAsync(HttpContext.User, cancellationToken))!.Id;
    }
}
