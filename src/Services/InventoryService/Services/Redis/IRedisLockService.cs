namespace Intchain.InventoryService.Services.Redis;

/// <summary>
/// Redis分布式锁服务接口
/// </summary>
public interface IRedisLockService
{
    /// <summary>
    /// 获取分布式锁
    /// </summary>
    /// <param name="key">锁的键名</param>
    /// <param name="timeout">超时时间</param>
    /// <returns>锁对象，如果获取失败返回null</returns>
    Task<IRedisLock?> AcquireLockAsync(string key, TimeSpan timeout);

    /// <summary>
    /// 在分布式锁保护下执行操作
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="key">锁的键名</param>
    /// <param name="operation">要执行的操作</param>
    /// <param name="timeout">超时时间</param>
    /// <returns>操作结果</returns>
    Task<T> ExecuteWithLockAsync<T>(string key, Func<Task<T>> operation, TimeSpan timeout);
}
