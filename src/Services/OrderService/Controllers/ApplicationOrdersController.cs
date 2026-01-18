using Intchain.OrderService.DTOs;
using Intchain.OrderService.Services;
using Microsoft.AspNetCore.Mvc;

namespace Intchain.OrderService.Controllers;

/// <summary>
/// 申请订单控制器
/// </summary>
[ApiController]
[Route("api/applicationorders")]
public class ApplicationOrdersController : ControllerBase
{
    private readonly IApplicationOrderService _applicationOrderService;

    public ApplicationOrdersController(IApplicationOrderService applicationOrderService)
    {
        _applicationOrderService = applicationOrderService;
    }

    /// <summary>
    /// 获取所有申请订单
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ApplicationOrderResponse>>> GetAllApplicationOrders()
    {
        var orders = await _applicationOrderService.GetAllApplicationOrdersAsync();
        return Ok(orders);
    }

    /// <summary>
    /// 根据ID获取申请订单
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApplicationOrderResponse>> GetApplicationOrder(int id)
    {
        var order = await _applicationOrderService.GetApplicationOrderAsync(id);

        if (order == null)
        {
            return NotFound(new { message = "订单不存在" });
        }

        return Ok(order);
    }

    /// <summary>
    /// 根据订单号获取申请订单
    /// </summary>
    [HttpGet("number/{orderNumber}")]
    public async Task<ActionResult<ApplicationOrderResponse>> GetApplicationOrderByNumber(string orderNumber)
    {
        var order = await _applicationOrderService.GetApplicationOrderByNumberAsync(orderNumber);

        if (order == null)
        {
            return NotFound(new { message = "订单不存在" });
        }

        return Ok(order);
    }

    /// <summary>
    /// 根据销售网点ID获取申请订单
    /// </summary>
    [HttpGet("sales-outlet/{salesOutletId}")]
    public async Task<ActionResult<List<ApplicationOrderResponse>>> GetApplicationOrdersBySalesOutlet(int salesOutletId)
    {
        var orders = await _applicationOrderService.GetApplicationOrdersBySalesOutletAsync(salesOutletId);
        return Ok(orders);
    }

    /// <summary>
    /// 根据彩票中心ID获取申请订单
    /// </summary>
    [HttpGet("lottery-center/{lotteryCenterId}")]
    public async Task<ActionResult<List<ApplicationOrderResponse>>> GetApplicationOrdersByLotteryCenter(int lotteryCenterId)
    {
        var orders = await _applicationOrderService.GetApplicationOrdersByLotteryCenterAsync(lotteryCenterId);
        return Ok(orders);
    }

    /// <summary>
    /// 根据状态获取申请订单
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<List<ApplicationOrderResponse>>> GetApplicationOrdersByStatus(string status)
    {
        var orders = await _applicationOrderService.GetApplicationOrdersByStatusAsync(status);
        return Ok(orders);
    }

    /// <summary>
    /// 创建申请订单
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApplicationOrderResponse>> CreateApplicationOrder([FromBody] CreateApplicationOrderRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var order = await _applicationOrderService.CreateApplicationOrderAsync(request);
            return CreatedAtAction(nameof(GetApplicationOrder), new { id = order!.Id }, order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 更新申请订单
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApplicationOrderResponse>> UpdateApplicationOrder(int id, [FromBody] UpdateApplicationOrderRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var order = await _applicationOrderService.UpdateApplicationOrderAsync(id, request);

        if (order == null)
        {
            return NotFound(new { message = "订单不存在" });
        }

        return Ok(order);
    }

    /// <summary>
    /// 删除申请订单
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteApplicationOrder(int id)
    {
        var result = await _applicationOrderService.DeleteApplicationOrderAsync(id);

        if (!result)
        {
            return NotFound(new { message = "订单不存在" });
        }

        return Ok(new { message = "订单删除成功" });
    }

    /// <summary>
    /// 审批通过申请订单
    /// </summary>
    [HttpPost("{id}/approve")]
    public async Task<ActionResult<OrderOperationResponse>> ApproveApplicationOrder(int id, [FromBody] ApproveApplicationOrderRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _applicationOrderService.ApproveApplicationOrderAsync(id, request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// 拒绝申请订单
    /// </summary>
    [HttpPost("{id}/reject")]
    public async Task<ActionResult<OrderOperationResponse>> RejectApplicationOrder(int id, [FromBody] RejectApplicationOrderRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _applicationOrderService.RejectApplicationOrderAsync(id, request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// 更新订单状态为已发货
    /// </summary>
    [HttpPost("{id}/ship")]
    public async Task<ActionResult<OrderOperationResponse>> ShipApplicationOrder(int id)
    {
        var result = await _applicationOrderService.UpdateToShippedAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// 完成申请订单
    /// </summary>
    [HttpPost("{id}/complete")]
    public async Task<ActionResult<OrderOperationResponse>> CompleteApplicationOrder(int id)
    {
        var result = await _applicationOrderService.CompleteApplicationOrderAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
