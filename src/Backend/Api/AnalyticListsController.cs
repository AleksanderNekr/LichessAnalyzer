using Backend.Auth;
using Backend.DataManagement.Users.Entities;
using Backend.DataManagement.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api;

[ApiController]
[Route("api")]
public class AnalyticListsController(
    AuthService           authService,
    AnalyticsListsService listsService) : Controller
{
    [HttpPost("lists")]
    [Authorize(AuthExtensions.LichessAuthPolicyName)]
    public async Task<ActionResult<AnalyticsList>> CreateByPlayers(
        [FromBody] string              name,
        [FromBody] ICollection<string> playersIds,
        CancellationToken              cancellationToken)
    {
        User? creator = await authService.GetCurrentUserAsync(HttpContext.User, cancellationToken);
        if (creator is null)
        {
            return Forbid();
        }

        AnalyticsList? list = await listsService.CreateByPlayersAsync(Guid.NewGuid(),
                                                                      name,
                                                                      creator,
                                                                      playersIds,
                                                                      cancellationToken);
        if (list is null)
        {
            return Conflict($"Max lists limit: {creator.MaxListsCount} reached! Unable to create");
        }

        return Ok(list);
    }
}
