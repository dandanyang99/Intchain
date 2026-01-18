namespace Intchain.OrderService.Utils;

/// <summary>
/// 订单号生成器
/// </summary>
public static class OrderNumberGenerator
{
    private static readonly object _lock = new object();
    private static int _sequenceNumber = 0;
    private static string _lastDate = string.Empty;

    /// <summary>
    /// 生成申请订单号
    /// 格式: APP-YYYYMMDD-XXXXX
    /// </summary>
    public static string GenerateApplicationOrderNumber()
    {
        lock (_lock)
        {
            var dateStr = DateTime.UtcNow.ToString("yyyyMMdd");

            // 如果日期变化，重置序列号
            if (_lastDate != dateStr)
            {
                _sequenceNumber = 0;
                _lastDate = dateStr;
            }

            _sequenceNumber = (_sequenceNumber + 1) % 100000;
            var sequence = _sequenceNumber.ToString("D5");
            return $"APP-{dateStr}-{sequence}";
        }
    }

    /// <summary>
    /// 生成印刷订单号
    /// 格式: PRINT-YYYYMMDD-XXXXX
    /// </summary>
    public static string GeneratePrintingOrderNumber()
    {
        lock (_lock)
        {
            var dateStr = DateTime.UtcNow.ToString("yyyyMMdd");

            // 如果日期变化，重置序列号
            if (_lastDate != dateStr)
            {
                _sequenceNumber = 0;
                _lastDate = dateStr;
            }

            _sequenceNumber = (_sequenceNumber + 1) % 100000;
            var sequence = _sequenceNumber.ToString("D5");
            return $"PRINT-{dateStr}-{sequence}";
        }
    }
}
