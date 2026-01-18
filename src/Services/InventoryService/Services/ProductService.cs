using Intchain.InventoryService.Data;
using Intchain.InventoryService.DTOs;
using Intchain.InventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace Intchain.InventoryService.Services;

/// <summary>
/// 产品服务实现
/// </summary>
public class ProductService : IProductService
{
    private readonly InventoryDbContext _context;

    public ProductService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<ProductResponse?> CreateProductAsync(CreateProductRequest request)
    {
        var product = new LotteryProduct
        {
            Name = request.Name,
            UnitPrice = request.UnitPrice,
            TotalStock = request.TotalStock,
            AvailableStock = request.TotalStock,
            ReservedStock = 0,
            LotteryCenterId = request.LotteryCenterId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.LotteryProducts.Add(product);
        await _context.SaveChangesAsync();

        return MapToResponse(product);
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
