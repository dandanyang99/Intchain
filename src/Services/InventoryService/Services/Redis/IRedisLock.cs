namespace Intchain.InventoryService.Services.Redis;

/// <summary>
/// Redis分布式锁接口
/// </summary>
public interface IRedisLock : IDisposable
{
    /// <summary>
    /// 锁的键名
    /// </summary>
    string LockKey { get; }

    /// <summary>
    /// 是否已获取锁
    /// </summary>
    bool IsAcquired { get; }

    /// <summary>
    /// 释放锁
    /// </summary>
    Task<bool> ReleaseAsync();
}
