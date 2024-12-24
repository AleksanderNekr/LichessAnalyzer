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
	// [Authorize(AuthExtensions.LichessAuthPolicyName)]
	public async Task<ActionResult<RatingForecastModel[]>> Predict([FromBody] RatingForecastModel[] ratings, int horizon)
	{
		var forecast = await _forecastService.GetForecastAsync(ratings, horizon);
		return Ok(forecast);
	}
}