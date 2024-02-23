﻿using Backend.Api.RequestModels;
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
    [HttpPost("players-lists")]
    [Authorize(AuthExtensions.LichessAuthPolicyName)]
    public async Task<ActionResult<AnalyticsList>> CreateByPlayers(
        [FromBody] CreateListRequestBodyModel requestBody,
        CancellationToken                     cancellationToken)
    {
        User? creator = await authService.GetCurrentUserAsync(HttpContext.User, cancellationToken);
        if (creator is null)
        {
            return Forbid();
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

        return Ok(list);
    }
}