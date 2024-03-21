using System.Net.Http.Json;
using Backend.DataManagement.LichessApi;
using Backend.DataManagement.LichessApi.ServiceResponsesModels;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace BackendTests.UnitTests;

public class LichessDataServiceTests
{
    private static HttpClient MoqHttpClient()
    {
        Mock<HttpMessageHandler> httpMsgMock = new();
        HttpResponseMessage responseMessage =
            new()
            {
                Content = JsonContent.Create(new List<object>
                                             {
                                                 new
                                                 {
                                                     id       = "id",
                                                     username = "test",
                                                     perfs = new Dictionary<string, object>
                                                             {
                                                                 {
                                                                     "bullet",
                                                                     new { games = 10, rating = 2000 }
                                                                 }
                                                             }
                                                 }
                                             })
            };

        httpMsgMock.Protected()
                   .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(responseMessage);

        return new HttpClient(httpMsgMock.Object) { BaseAddress = new Uri("http://localhost") };
    }

    private static ILogger<T> MoqLogger<T>()
    {
        return new Mock<ILogger<T>>().Object;
    }

    [Fact]
    public async Task Test_GetChessPlayers_ShouldReturnObjects()
    {
        // Arrange
        HttpClient                  client = MoqHttpClient();
        ILogger<LichessDataService> logger = MoqLogger<LichessDataService>();
        LichessDataService          sut    = new(client, logger);
        IEnumerable<PlayerResponse> valid =
        [
            new PlayerResponse("test", "id", [ ], [ ], [ ], [ ], [ ])
        ];

        // Act
        IEnumerable<PlayerResponse> response = await sut.GetChessPlayersAsync([ "id" ]);

        // Assert
        Assert.Equal(valid, response);
    }
}
