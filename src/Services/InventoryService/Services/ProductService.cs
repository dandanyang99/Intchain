using Intchain.InventoryService.Data;
using Intchain.InventoryService.DTOs;
using Intchain.InventoryService.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace Intchain.InventoryService.Services;

/// <summary>
/// 产品服务实现
/// </summary>
public class ProductService : IProductService
{
    private readonly InventoryDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public ProductService(InventoryDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ProductResponse?> CreateProductAsync(CreateProductRequest request)
    {
        var product = new LotteryProduct
        {
            Name = request.Name,
            UnitPrice = request.UnitPrice,
            TotalStock = request.TotalStock,
            AvailableStock = 0, // 初始库存为0，印刷完成后才更新为实际库存
            ReservedStock = 0,
            LotteryCenterId = request.LotteryCenterId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.LotteryProducts.Add(product);
        await _context.SaveChangesAsync();

        // 发布产品的同时，创建印刷订单发给印刷厂
        try
        {
            await CreatePrintingOrderAsync(product.Id, request.PrintingFactoryId, request.TotalStock);
        }
        catch (Exception ex)
        {
            // 记录错误但不影响产品创建
            Console.WriteLine($"创建印刷订单失败: {ex.Message}");
        }

        return MapToResponse(product);
    }

    private async Task CreatePrintingOrderAsync(int productId, int printingFactoryId, int quantity)
    {
        var httpClient = _httpClientFactory.CreateClient("OrderService");

        var printingOrderRequest = new
        {
            ApplicationOrderId = (int?)null,
            PrintingFactoryId = printingFactoryId,
            ProductId = productId,
            Quantity = quantity,
            Remarks = "Product publication printing order"
        };

        var jsonContent = JsonSerializer.Serialize(printingOrderRequest);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("/api/printingorders", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task<ProductResponse?> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        var product = await _context.LotteryProducts.FindAsync(id);

        if (product == null)
        {
            return null;
        }

        product.Name = request.Name;
        product.UnitPrice = request.UnitPrice;
        product.TotalStock = request.TotalStock;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponse(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.LotteryProducts.FindAsync(id);

        if (product == null)
        {
            return false;
        }

        _context.LotteryProducts.Remove(product);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<ProductResponse?> GetProductAsync(int id)
    {
        var product = await _context.LotteryProducts.FindAsync(id);

        return product == null ? null : MapToResponse(product);
    }

    public async Task<List<ProductResponse>> GetAllProductsAsync()
    {
        var products = await _context.LotteryProducts.ToListAsync();

        return products.Select(MapToResponse).ToList();
    }

    public async Task<List<ProductResponse>> GetProductsByLotteryCenterAsync(int lotteryCenterId)
    {
        var products = await _context.LotteryProducts
            .Where(p => p.LotteryCenterId == lotteryCenterId)
            .ToListAsync();

        return products.Select(MapToResponse).ToList();
    }

    private static ProductResponse MapToResponse(LotteryProduct product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            UnitPrice = product.UnitPrice,
            TotalStock = product.TotalStock,
            AvailableStock = product.AvailableStock,
            ReservedStock = product.ReservedStock,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}
