using Intchain.InventoryService.DTOs;
using Intchain.InventoryService.Services;
using Microsoft.AspNetCore.Mvc;

namespace Intchain.InventoryService.Controllers;

/// <summary>
/// 库存管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>
    /// 预留库存
    /// </summary>
    [HttpPost("reserve")]
    public async Task<IActionResult> ReserveInventory([FromBody] ReserveInventoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _inventoryService.ReserveInventoryAsync(
                request.ProductId,
                request.Quantity,
                request.OrderId
            );

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 释放预留库存
    /// </summary>
    [HttpPost("release")]
    public async Task<IActionResult> ReleaseInventory([FromBody] ReleaseInventoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _inventoryService.ReleaseReservedInventoryAsync(
                request.ProductId,
                request.Quantity,
                request.OrderId
            );

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 确认库存扣减
    /// </summary>
    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmInventory([FromBody] ConfirmInventoryRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _inventoryService.ConfirmInventoryDeductionAsync(
                request.ProductId,
                request.Quantity,
                request.OrderId
            );

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 获取库存统计
    /// </summary>
    [HttpGet("stats/{productId}")]
    public async Task<IActionResult> GetInventoryStats(int productId)
    {
        try
        {
            var stats = await _inventoryService.GetInventoryStatsAsync(productId);
            return Ok(stats);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
