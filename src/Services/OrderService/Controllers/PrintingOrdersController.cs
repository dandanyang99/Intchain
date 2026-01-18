using Intchain.OrderService.DTOs;
using Intchain.OrderService.Services;
using Microsoft.AspNetCore.Mvc;

namespace Intchain.OrderService.Controllers;

/// <summary>
/// 印刷订单控制器
/// </summary>
[ApiController]
[Route("api/printingorders")]
public class PrintingOrdersController : ControllerBase
{
    private readonly IPrintingOrderService _printingOrderService;

    public PrintingOrdersController(IPrintingOrderService printingOrderService)
    {
        _printingOrderService = printingOrderService;
    }

    /// <summary>
    /// 获取所有印刷订单
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<PrintingOrderResponse>>> GetAllPrintingOrders()
    {
        var orders = await _printingOrderService.GetAllPrintingOrdersAsync();
        return Ok(orders);
    }

    /// <summary>
    /// 根据ID获取印刷订单
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<PrintingOrderResponse>> GetPrintingOrder(int id)
    {
        var order = await _printingOrderService.GetPrintingOrderAsync(id);

        if (order == null)
        {
            return NotFound(new { message = "印刷订单不存在" });
        }

        return Ok(order);
    }

    /// <summary>
    /// 根据订单号获取印刷订单
    /// </summary>
    [HttpGet("number/{orderNumber}")]
    public async Task<ActionResult<PrintingOrderResponse>> GetPrintingOrderByNumber(string orderNumber)
    {
        var order = await _printingOrderService.GetPrintingOrderByNumberAsync(orderNumber);

        if (order == null)
        {
            return NotFound(new { message = "印刷订单不存在" });
        }

        return Ok(order);
    }

    /// <summary>
    /// 根据印刷厂ID获取印刷订单
    /// </summary>
    [HttpGet("factory/{printingFactoryId}")]
    public async Task<ActionResult<List<PrintingOrderResponse>>> GetPrintingOrdersByFactory(int printingFactoryId)
    {
        var orders = await _printingOrderService.GetPrintingOrdersByFactoryAsync(printingFactoryId);
        return Ok(orders);
    }

    /// <summary>
    /// 根据状态获取印刷订单
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<List<PrintingOrderResponse>>> GetPrintingOrdersByStatus(string status)
    {
        var orders = await _printingOrderService.GetPrintingOrdersByStatusAsync(status);
        return Ok(orders);
    }

    /// <summary>
    /// 根据申请订单ID获取印刷订单
    /// </summary>
    [HttpGet("application/{applicationOrderId}")]
    public async Task<ActionResult<PrintingOrderResponse>> GetPrintingOrderByApplicationOrder(int applicationOrderId)
    {
        var order = await _printingOrderService.GetPrintingOrderByApplicationOrderIdAsync(applicationOrderId);

        if (order == null)
        {
            return NotFound(new { message = "印刷订单不存在" });
        }

        return Ok(order);
    }

    /// <summary>
    /// 创建印刷订单
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<PrintingOrderResponse>> CreatePrintingOrder([FromBody] CreatePrintingOrderRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var order = await _printingOrderService.CreatePrintingOrderAsync(request);
        return CreatedAtAction(nameof(GetPrintingOrder), new { id = order!.Id }, order);
    }

    /// <summary>
    /// 印刷厂接受订单
    /// </summary>
    [HttpPost("{id}/accept")]
    public async Task<ActionResult<OrderOperationResponse>> AcceptPrintingOrder(int id)
    {
        var result = await _printingOrderService.AcceptPrintingOrderAsync(id);

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
    public async Task<ActionResult<OrderOperationResponse>> ShipPrintingOrder(int id)
    {
        var result = await _printingOrderService.UpdateToShippedAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// 完成印刷订单
    /// </summary>
    [HttpPost("{id}/complete")]
    public async Task<ActionResult<OrderOperationResponse>> CompletePrintingOrder(int id)
    {
        var result = await _printingOrderService.CompletePrintingOrderAsync(id);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
