using Backend.Api.RequestModels;
using Backend.Auth;
using Backend.MLGrpc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Api;

[ApiController]
[Route("api")]
public class RatingsForecastController : Controller
{
	private readonly ForecastGrpcService _forecastService;

	public RatingsForecastController(ForecastGrpcService forecastService)
	{
		_forecastService = forecastService;
	}

	[HttpPost("ratings-forecast")]
	[Authorize(AuthExtensions.LichessAuthPolicyName)]
	public async Task<ActionResult<ForecastApiModel[]>> Predict([FromBody] ForecastApiModel request)
	{
		var forecast = await _forecastService.GetForecastAsync(request.Ratings, request.Horizon);
		return Ok(forecast);
	}

	public sealed record ForecastApiModel(RatingForecastModel[] Ratings, int Horizon);
}