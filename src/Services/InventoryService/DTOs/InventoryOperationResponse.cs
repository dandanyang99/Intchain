namespace Intchain.InventoryService.DTOs;

/// <summary>
/// 库存操作响应
/// </summary>
public class InventoryOperationResponse
{
    /// <summary>
    /// 操作是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 当前可用库存
    /// </summary>
    public int CurrentAvailableStock { get; set; }

    /// <summary>
    /// 当前预留库存
    /// </summary>
    public int CurrentReservedStock { get; set; }
}
