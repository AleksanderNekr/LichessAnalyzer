using Backend.Auth;
using Backend.DataManagement.LichessApi;
using Backend.DataManagement.LichessApi.ServiceResponsesModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api;

[ApiController]
[Route("api")]
public class GetDataController(CachedDataService cachedDataService) : Controller
{
    [HttpGet("players")]
    [Authorize(AuthExtensions.LichessAuthPolicyName)]
    public async Task<ActionResult<IEnumerable<PlayerResponse>>> GetPlayersInfo(
        [FromQuery] List<string>               ids,
        [FromQuery] IEnumerable<PlayerStat>?   withStats         = null,
        [FromQuery] IEnumerable<PlayCategory>? withCategories    = null,
        CancellationToken                      cancellationToken = default)
    {
        List<PlayerStat>   stats      = (withStats      ?? Enumerable.Empty<PlayerStat>()).ToList();
        List<PlayCategory> categories = (withCategories ?? Enum.GetValues<PlayCategory>()).ToList();
        IEnumerable<PlayerResponse> players = await cachedDataService.GetChessPlayersAsync(ids,
                                                                                           stats,
                                                                                           categories,
                                                                                           cancellationToken);

        return Ok(players);
    }


    [HttpGet("teams")]
    [Authorize(AuthExtensions.LichessAuthPolicyName)]
    public async Task<ActionResult<IEnumerable<TeamResponse>>> GetTeamsInfo(
        [FromQuery] List<string> ids,
        [FromQuery] bool         withParticipants  = false,
        [FromQuery] bool         withTournaments   = false,
        CancellationToken        cancellationToken = default)
    {
        IEnumerable<TeamResponse> teams = await cachedDataService.GetTeamsAsync(ids,
                                                                                withParticipants,
                                                                                withTournaments,
                                                                                cancellationToken);

        return Ok(teams);
    }
}
