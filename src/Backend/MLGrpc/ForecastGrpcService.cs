using Backend.Api.RequestModels;
using Grpc.Net.Client;
using LichessAnalyzerForecast;

namespace Backend.MLGrpc;

public sealed class ForecastGrpcService
{
	private readonly RatingPredictor.RatingPredictorClient _client;

	public ForecastGrpcService()
	{
		var channel = GrpcChannel.ForAddress("http://ml:5000");
		_client = new RatingPredictor.RatingPredictorClient(channel);
	}

	public async Task<RatingForecastModel[]> GetForecastAsync(RatingForecastModel[] ratings, int horizon)
	{
		var request = new PredictRatingRequest
		{
			Ratings =
			{
				ratings.Select(r => new Rating
				{
					Date = r.Date.ToString("yyyy-MM-dd"),
					Rating_ = r.Rating,
				}),
			},
			NumPredictions = horizon,
		};

		var response = await _client.PredictRatingAsync(request);
		return response.Predictions.Select(p => new RatingForecastModel(Date: DateOnly.Parse(p.Date), Rating: p.Rating_)).ToArray();
	}
}