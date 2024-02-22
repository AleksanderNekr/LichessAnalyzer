using System.Runtime.CompilerServices;
using Backend.DataManagement.LichessApi;
using Backend.DataManagement.LichessApi.ServiceResponsesModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api;

[ApiController]
[Route("api")]
public class GetDataController(DataService dataService) : Controller
{
    [HttpGet("players")]
    public async Task<ActionResult<IEnumerable<PlayerResponse>>> GetPlayersInfo(
        [FromQuery] IEnumerable<string>        ids,
        [FromQuery] IEnumerable<PlayerStat>?   withStats         = null,
        [FromQuery] IEnumerable<PlayCategory>? withCategories    = null,
        CancellationToken                      cancellationToken = default)
    {
        List<PlayerStat>   stats      = (withStats      ?? Enumerable.Empty<PlayerStat>()).ToList();
        List<PlayCategory> categories = (withCategories ?? Enum.GetValues<PlayCategory>()).ToList();
        IEnumerable<PlayerResponse>? players = await dataService.GetChessPlayersAsync(ids,
                                                                                     stats,
                                                                                     categories,
                                                                                     cancellationToken);

        return Ok(players);
    }


    [HttpGet("teams")]
    public Task<ActionResult<IEnumerable<TeamResponse>>> GetTeamsInfo(
        [FromQuery] IEnumerable<string> ids,
        [FromQuery] bool                withParticipants = false,
        [FromQuery] bool                withTournaments  = false)
    {
        IEnumerable<TeamResponse> teams = dataService.GetTeams(ids,
                                                               withParticipants,
                                                               withTournaments);

        return Task.FromResult<ActionResult<IEnumerable<TeamResponse>>>(Ok(teams));
    }
}
