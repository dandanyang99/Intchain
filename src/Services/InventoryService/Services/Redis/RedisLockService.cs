using StackExchange.Redis;

namespace Intchain.InventoryService.Services.Redis;

/// <summary>
/// Redis分布式锁服务实现
/// </summary>
public class RedisLockService : IRedisLockService
{
    private readonly IConnectionMultiplexer _redis;

    public RedisLockService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<IRedisLock?> AcquireLockAsync(string key, TimeSpan timeout)
    {
        var database = _redis.GetDatabase();
        var lockValue = Guid.NewGuid().ToString();
        var expiry = timeout;

        // 尝试获取锁
        var acquired = await database.StringSetAsync(
            key,
            lockValue,
            expiry,
            When.NotExists
        );

        if (acquired)
        {
            return new RedisLock(database, key, lockValue, true);
        }

        return null;
    }

    public async Task<T> ExecuteWithLockAsync<T>(string key, Func<Task<T>> operation, TimeSpan timeout)
    {
        var lockAcquired = await AcquireLockAsync(key, timeout);

        if (lockAcquired == null)
        {
            throw new InvalidOperationException($"无法获取锁: {key}");
        }

        try
        {
            return await operation();
        }
        finally
        {
            await lockAcquired.ReleaseAsync();
            lockAcquired.Dispose();
        }
    }
}
