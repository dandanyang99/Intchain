using Intchain.InventoryService.Data;
using Intchain.InventoryService.DTOs;
using Intchain.InventoryService.Services.Redis;
using Microsoft.EntityFrameworkCore;

namespace Intchain.InventoryService.Services;

/// <summary>
/// 库存服务实现
/// </summary>
public class InventoryService : IInventoryService
{
    private readonly InventoryDbContext _context;
    private readonly IRedisLockService _redisLockService;

    public InventoryService(InventoryDbContext context, IRedisLockService redisLockService)
    {
        _context = context;
        _redisLockService = redisLockService;
    }

    public async Task<ProductResponse?> GetProductAsync(int productId)
    {
        var product = await _context.LotteryProducts.FindAsync(productId);

        if (product == null)
        {
            return null;
        }

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

    public async Task<List<ProductResponse>> GetProductsByLotteryCenterAsync(int lotteryCenterId)
    {
        var products = await _context.LotteryProducts
            .Where(p => p.LotteryCenterId == lotteryCenterId)
            .ToListAsync();

        return products.Select(p => new ProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            UnitPrice = p.UnitPrice,
            TotalStock = p.TotalStock,
            AvailableStock = p.AvailableStock,
            ReservedStock = p.ReservedStock,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();
    }

    public async Task<InventoryOperationResponse> ReserveInventoryAsync(int productId, int quantity, string orderId)
    {
        var lockKey = $"inventory:lock:{productId}";

        return await _redisLockService.ExecuteWithLockAsync(lockKey, async () =>
        {
            var product = await _context.LotteryProducts.FindAsync(productId);

            if (product == null)
            {
                return new InventoryOperationResponse
                {
                    Success = false,
                    Message = "产品不存在"
                };
            }

            if (product.AvailableStock < quantity)
            {
                return new InventoryOperationResponse
                {
                    Success = false,
                    Message = "库存不足",
                    CurrentAvailableStock = product.AvailableStock,
                    CurrentReservedStock = product.ReservedStock
                };
            }

            product.AvailableStock -= quantity;
            product.ReservedStock += quantity;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new InventoryOperationResponse
            {
                Success = true,
                Message = "库存预留成功",
                CurrentAvailableStock = product.AvailableStock,
                CurrentReservedStock = product.ReservedStock
            };
        }, TimeSpan.FromSeconds(5));
    }

    public async Task<InventoryOperationResponse> ReleaseReservedInventoryAsync(int productId, int quantity, string orderId)
    {
        var lockKey = $"inventory:lock:{productId}";

        return await _redisLockService.ExecuteWithLockAsync(lockKey, async () =>
        {
            var product = await _context.LotteryProducts.FindAsync(productId);

            if (product == null)
            {
                return new InventoryOperationResponse
                {
                    Success = false,
                    Message = "产品不存在"
                };
            }

            if (product.ReservedStock < quantity)
            {
                return new InventoryOperationResponse
                {
                    Success = false,
                    Message = "预留库存不足",
                    CurrentAvailableStock = product.AvailableStock,
                    CurrentReservedStock = product.ReservedStock
                };
            }

            product.ReservedStock -= quantity;
            product.AvailableStock += quantity;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new InventoryOperationResponse
            {
                Success = true,
                Message = "库存释放成功",
                CurrentAvailableStock = product.AvailableStock,
                CurrentReservedStock = product.ReservedStock
            };
        }, TimeSpan.FromSeconds(5));
    }

    public async Task<InventoryOperationResponse> ConfirmInventoryDeductionAsync(int productId, int quantity, string orderId)
    {
        var lockKey = $"inventory:lock:{productId}";

        return await _redisLockService.ExecuteWithLockAsync(lockKey, async () =>
        {
            var product = await _context.LotteryProducts.FindAsync(productId);

            if (product == null)
            {
                return new InventoryOperationResponse
                {
                    Success = false,
                    Message = "产品不存在"
                };
            }

            if (product.ReservedStock < quantity)
            {
                return new InventoryOperationResponse
                {
                    Success = false,
                    Message = "预留库存不足",
                    CurrentAvailableStock = product.AvailableStock,
                    CurrentReservedStock = product.ReservedStock
                };
            }

            product.ReservedStock -= quantity;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new InventoryOperationResponse
            {
                Success = true,
                Message = "库存扣减确认成功",
                CurrentAvailableStock = product.AvailableStock,
                CurrentReservedStock = product.ReservedStock
            };
        }, TimeSpan.FromSeconds(5));
    }

    public async Task<InventoryStatsResponse> GetInventoryStatsAsync(int productId)
    {
        var product = await _context.LotteryProducts.FindAsync(productId);

        if (product == null)
        {
            throw new InvalidOperationException("产品不存在");
        }

        var soldStock = product.TotalStock - product.AvailableStock - product.ReservedStock;

        return new InventoryStatsResponse
        {
            ProductId = product.Id,
            TotalStock = product.TotalStock,
            AvailableStock = product.AvailableStock,
            ReservedStock = product.ReservedStock,
            SoldStock = soldStock
        };
    }
}
