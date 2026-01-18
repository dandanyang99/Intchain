using Intchain.InventoryService.DTOs;
using Intchain.InventoryService.Services;
using Microsoft.AspNetCore.Mvc;

namespace Intchain.InventoryService.Controllers;

/// <summary>
/// 产品管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// 获取所有产品
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    /// <summary>
    /// 根据ID获取产品
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetProductAsync(id);

        if (product == null)
        {
            return NotFound(new { message = "产品不存在" });
        }

        return Ok(product);
    }

    /// <summary>
    /// 根据彩票中心ID获取产品
    /// </summary>
    [HttpGet("lottery-center/{lotteryCenterId}")]
    public async Task<IActionResult> GetProductsByLotteryCenter(int lotteryCenterId)
    {
        var products = await _productService.GetProductsByLotteryCenterAsync(lotteryCenterId);
        return Ok(products);
    }

    /// <summary>
    /// 创建产品
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var product = await _productService.CreateProductAsync(request);

        if (product == null)
        {
            return BadRequest(new { message = "创建产品失败" });
        }

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    /// <summary>
    /// 更新产品
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var product = await _productService.UpdateProductAsync(id, request);

        if (product == null)
        {
            return NotFound(new { message = "产品不存在" });
        }

        return Ok(product);
    }

    /// <summary>
    /// 删除产品
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);

        if (!result)
        {
            return NotFound(new { message = "产品不存在" });
        }

        return Ok(new { message = "产品删除成功" });
    }
}
