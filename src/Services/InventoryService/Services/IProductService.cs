using Intchain.InventoryService.DTOs;

namespace Intchain.InventoryService.Services;

/// <summary>
/// 产品服务接口
/// </summary>
public interface IProductService
{
    /// <summary>
    /// 创建产品
    /// </summary>
    Task<ProductResponse?> CreateProductAsync(CreateProductRequest request);

    /// <summary>
    /// 更新产品
    /// </summary>
    Task<ProductResponse?> UpdateProductAsync(int id, UpdateProductRequest request);

    /// <summary>
    /// 删除产品
    /// </summary>
    Task<bool> DeleteProductAsync(int id);

    /// <summary>
    /// 获取产品
    /// </summary>
    Task<ProductResponse?> GetProductAsync(int id);

    /// <summary>
    /// 获取所有产品
    /// </summary>
    Task<List<ProductResponse>> GetAllProductsAsync();

    /// <summary>
    /// 根据彩票中心ID获取产品
    /// </summary>
    Task<List<ProductResponse>> GetProductsByLotteryCenterAsync(int lotteryCenterId);
}
