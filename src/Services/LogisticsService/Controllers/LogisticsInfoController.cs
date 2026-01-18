using Intchain.LogisticsService.DTOs;
using Intchain.LogisticsService.Services;
using Microsoft.AspNetCore.Mvc;

namespace Intchain.LogisticsService.Controllers
{
    /// <summary>
    /// 物流信息控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LogisticsInfoController : ControllerBase
    {
        private readonly ILogisticsService _logisticsService;

        public LogisticsInfoController(ILogisticsService logisticsService)
        {
            _logisticsService = logisticsService;
        }

        /// <summary>
        /// 获取所有物流信息
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllLogisticsInfo()
        {
            var result = await _logisticsService.GetAllLogisticsInfoAsync();
            return Ok(result);
        }

        /// <summary>
        /// 根据ID获取物流信息
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLogisticsInfo(int id)
        {
            var result = await _logisticsService.GetLogisticsInfoAsync(id);

            if (result == null)
                return NotFound(new { message = "物流信息不存在" });

            return Ok(result);
        }

        /// <summary>
        /// 根据物流单号获取物流信息
        /// </summary>
        [HttpGet("tracking/{trackingNumber}")]
        public async Task<IActionResult> GetLogisticsInfoByTrackingNumber(string trackingNumber)
        {
            var result = await _logisticsService.GetLogisticsInfoByTrackingNumberAsync(trackingNumber);

            if (result == null)
                return NotFound(new { message = "物流信息不存在" });

            return Ok(result);
        }

        /// <summary>
        /// 根据印刷订单ID获取物流信息
        /// </summary>
        [HttpGet("order/{printingOrderId}")]
        public async Task<IActionResult> GetLogisticsInfoByPrintingOrderId(int printingOrderId)
        {
            var result = await _logisticsService.GetLogisticsInfoByPrintingOrderIdAsync(printingOrderId);

            if (result == null)
                return NotFound(new { message = "物流信息不存在" });

            return Ok(result);
        }

        /// <summary>
        /// 根据状态获取物流信息列表
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetLogisticsInfoByStatus(string status)
        {
            var result = await _logisticsService.GetLogisticsInfoByStatusAsync(status);
            return Ok(result);
        }

        /// <summary>
        /// 创建物流信息
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateLogisticsInfo([FromBody] CreateLogisticsInfoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _logisticsService.CreateLogisticsInfoAsync(request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(result);
        }

        /// <summary>
        /// 更新物流状态
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateLogisticsStatus(int id, [FromBody] UpdateLogisticsStatusRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _logisticsService.UpdateLogisticsStatusAsync(id, request);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(result);
        }

        /// <summary>
        /// 删除物流信息
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLogisticsInfo(int id)
        {
            var result = await _logisticsService.DeleteLogisticsInfoAsync(id);

            if (!result)
                return NotFound(new { message = "物流信息不存在" });

            return Ok(new { message = "删除成功" });
        }
    }
}
