using System.Text.Json;
using Backend.DataManagement.Users.Entities;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Backend.DataManagement.Analytics;

public class RedisAnalyticsCacheService : IAnalyticsCacheService
{
    private readonly ILogger<RedisAnalyticsCacheService> _logger;
    private readonly IDatabase                           _db;

    public RedisAnalyticsCacheService(IOptions<CacheOptions> options, ILogger<RedisAnalyticsCacheService> logger)
    {
        _logger = logger;
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options.Value.RedisConnectionsString);
        _db = redis.GetDatabase();

        _logger.LogDebug("Connected to Redis");
    }

    public async Task<bool> CachePlayerAsync(Player player)
    {
        var    key   = $"player:{player.Id}";
        string value = JsonSerializer.Serialize(player);

        bool isSet = await _db.StringSetAsync(key, value);

        if (isSet)
        {
            _logger.LogDebug("Successfully set Player at key: {Key}\nValue: {Value}", key, value);
        }
        else
        {
            _logger.LogDebug("Unsuccessfully try to set Player at key: {Key}\nValue: {Value}", key, value);
        }

        return isSet;
    }

    public async Task<Player?> ExtractPlayerAsync(string playerId)
    {
        var key = $"player:{playerId}";
        _logger.LogDebug("Trying to get Player by: {Key}", key);
        RedisValue value = await _db.StringGetAsync(key);

        if (value.HasValue)
        {
            var result = JsonSerializer.Deserialize<Player>(value.ToString());
            _logger.LogDebug("Deserialized Value: {@Value}", result);

            return result;
        }

        _logger.LogDebug("There's no value at key: {Key}", key);

        return null;
    }

    public async Task<bool> CacheTeamAsync(Team team)
    {
        var    key   = $"team:{team.Id}";
        string value = JsonSerializer.Serialize(team);

        bool isSet = await _db.StringSetAsync(key, value);

        if (isSet)
        {
            _logger.LogDebug("Successfully set Team at key: {Key}\nValue: {Value}", key, value);
        }
        else
        {
            _logger.LogDebug("Unsuccessfully try to set Team at key: {Key}\nValue: {Value}", key, value);
        }

        return isSet;
    }

    public async Task<Team?> ExtractTeamAsync(string playerId)
    {
        var key = $"team:{playerId}";
        _logger.LogDebug("Trying to get Team by: {Key}", key);
        RedisValue value = await _db.StringGetAsync(key);

        if (value.HasValue)
        {
            var result = JsonSerializer.Deserialize<Team>(value.ToString());
            _logger.LogDebug("Deserialized Value: {@Value}", result);

            return result;
        }

        _logger.LogDebug("There's no value at key: {Key}", key);

        return null;
    }
}
