using Backend.DataManagement.LichessApi;
using Backend.DataManagement.LichessApi.ServiceResponsesModels;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api;

[ApiController]
[Route("api")]
public class GetDataController(GetDataService getDataService) : Controller
{
    [HttpPost("players-info")]
    public async Task<ActionResult<IEnumerable<PlayerResponse>>> GetPlayersInfo([FromBody] IEnumerable<string> ids,
                                                                                CancellationToken              cancellationToken)
    {
        return Ok(await getDataService.GetChessPlayersAsync(ids, cancellationToken));
    }
}
