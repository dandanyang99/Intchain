using StackExchange.Redis;

namespace Intchain.InventoryService.Services.Redis;

/// <summary>
/// Redis分布式锁实现
/// </summary>
public class RedisLock : IRedisLock
{
    private readonly IDatabase _database;
    private readonly string _lockKey;
    private readonly string _lockValue;
    private bool _isAcquired;
    private bool _disposed;

    public string LockKey => _lockKey;
    public bool IsAcquired => _isAcquired;

    public RedisLock(IDatabase database, string lockKey, string lockValue, bool isAcquired)
    {
        _database = database;
        _lockKey = lockKey;
        _lockValue = lockValue;
        _isAcquired = isAcquired;
    }

    public async Task<bool> ReleaseAsync()
    {
        if (!_isAcquired || _disposed)
        {
            return false;
        }

        try
        {
            // 使用Lua脚本确保只释放自己持有的锁
            var script = @"
                if redis.call('get', KEYS[1]) == ARGV[1] then
                    return redis.call('del', KEYS[1])
                else
                    return 0
                end";

            var result = await _database.ScriptEvaluateAsync(
                script,
                new RedisKey[] { _lockKey },
                new RedisValue[] { _lockValue }
            );

            _isAcquired = false;
            return (int)result == 1;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        if (_isAcquired)
        {
            // 同步释放锁
            ReleaseAsync().GetAwaiter().GetResult();
        }

        _disposed = true;
    }
}
