using Intchain.UserService.DTOs;
using Intchain.UserService.Services;
using Microsoft.AspNetCore.Mvc;

namespace Intchain.UserService.Controllers;

/// <summary>
/// 销售网点控制器
/// </summary>
[ApiController]
[Route("api/salesoutlets")]
public class SalesOutletController : ControllerBase
{
    private readonly ISalesOutletService _salesOutletService;

    public SalesOutletController(ISalesOutletService salesOutletService)
    {
        _salesOutletService = salesOutletService;
    }

    /// <summary>
    /// 创建销售网点（同时创建默认用户）
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SalesOutletResponse>> CreateSalesOutlet([FromBody] CreateSalesOutletRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var response = await _salesOutletService.CreateSalesOutletAsync(request);
            return CreatedAtAction(nameof(GetSalesOutlet), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 批量创建销售网点（同时为每个网点创建默认用户）
    /// </summary>
    [HttpPost("batch")]
    public async Task<ActionResult<List<SalesOutletResponse>>> BatchCreateSalesOutlets([FromBody] BatchCreateSalesOutletsRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var responses = await _salesOutletService.BatchCreateSalesOutletsAsync(request);
            return Ok(responses);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 获取所有销售网点
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<SalesOutletResponse>>> GetAllSalesOutlets()
    {
        var salesOutlets = await _salesOutletService.GetAllSalesOutletsAsync();
        return Ok(salesOutlets);
    }

    /// <summary>
    /// 根据ID获取销售网点
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SalesOutletResponse>> GetSalesOutlet(int id)
    {
        var salesOutlet = await _salesOutletService.GetSalesOutletAsync(id);

        if (salesOutlet == null)
        {
            return NotFound(new { message = "销售网点不存在" });
        }

        return Ok(salesOutlet);
    }

    /// <summary>
    /// 根据彩票中心ID获取销售网点
    /// </summary>
    [HttpGet("lotterycenter/{lotteryCenterId}")]
    public async Task<ActionResult<List<SalesOutletResponse>>> GetSalesOutletsByLotteryCenter(int lotteryCenterId)
    {
        var salesOutlets = await _salesOutletService.GetSalesOutletsByLotteryCenterIdAsync(lotteryCenterId);
        return Ok(salesOutlets);
    }

    /// <summary>
    /// 更新销售网点
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<SalesOutletResponse>> UpdateSalesOutlet(int id, [FromBody] UpdateSalesOutletRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var response = await _salesOutletService.UpdateSalesOutletAsync(id, request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// 删除销售网点
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSalesOutlet(int id)
    {
        var result = await _salesOutletService.DeleteSalesOutletAsync(id);

        if (!result)
        {
            return NotFound(new { message = "销售网点不存在" });
        }

        return Ok(new { message = "销售网点删除成功" });
    }
}
