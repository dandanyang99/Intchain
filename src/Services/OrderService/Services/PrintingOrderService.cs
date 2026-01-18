using Intchain.OrderService.Data;
using Intchain.OrderService.DTOs;
using Intchain.OrderService.Models;
using Intchain.OrderService.Utils;
using Intchain.OrderService.Constants;
using Microsoft.EntityFrameworkCore;

namespace Intchain.OrderService.Services;

/// <summary>
/// 印刷订单服务实现
/// </summary>
public class PrintingOrderService : IPrintingOrderService
{
    private readonly OrderDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public PrintingOrderService(OrderDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<PrintingOrderResponse?> CreatePrintingOrderAsync(CreatePrintingOrderRequest request)
    {
        var order = new PrintingOrder
        {
            OrderNumber = OrderNumberGenerator.GeneratePrintingOrderNumber(),
            ApplicationOrderId = request.ApplicationOrderId,
            PrintingFactoryId = request.PrintingFactoryId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            Status = OrderStatus.PrintingPending,
            Remarks = request.Remarks,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.PrintingOrders.Add(order);
        await _context.SaveChangesAsync();

        return MapToResponse(order);
    }

    public async Task<PrintingOrderResponse?> GetPrintingOrderAsync(int id)
    {
        var order = await _context.PrintingOrders.FindAsync(id);

        return order == null ? null : MapToResponse(order);
    }

    public async Task<PrintingOrderResponse?> GetPrintingOrderByNumberAsync(string orderNumber)
    {
        var order = await _context.PrintingOrders
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        return order == null ? null : MapToResponse(order);
    }

    public async Task<PrintingOrderResponse?> GetPrintingOrderByApplicationOrderIdAsync(int applicationOrderId)
    {
        var order = await _context.PrintingOrders
            .FirstOrDefaultAsync(o => o.ApplicationOrderId == applicationOrderId);

        return order == null ? null : MapToResponse(order);
    }

    public async Task<List<PrintingOrderResponse>> GetAllPrintingOrdersAsync()
    {
        var orders = await _context.PrintingOrders.ToListAsync();

        return orders.Select(MapToResponse).ToList();
    }

    public async Task<List<PrintingOrderResponse>> GetPrintingOrdersByFactoryAsync(int printingFactoryId)
    {
        var orders = await _context.PrintingOrders
            .Where(o => o.PrintingFactoryId == printingFactoryId)
            .ToListAsync();

        return orders.Select(MapToResponse).ToList();
    }

    public async Task<List<PrintingOrderResponse>> GetPrintingOrdersByStatusAsync(string status)
    {
        var orders = await _context.PrintingOrders
            .Where(o => o.Status == status)
            .ToListAsync();

        return orders.Select(MapToResponse).ToList();
    }

    // Placeholder methods for Phase 3 implementation
    public async Task<OrderOperationResponse> AcceptPrintingOrderAsync(int id)
    {
        return await UpdateOrderStatusAsync(id, OrderStatus.PrintingInProduction);
    }

    public async Task<OrderOperationResponse> UpdateToWaitingShipmentAsync(int id)
    {
        return await UpdateOrderStatusAsync(id, OrderStatus.PrintingWaitingShipment);
    }

    public async Task<OrderOperationResponse> UpdateToShippedAsync(int id)
    {
        return await UpdateOrderStatusAsync(id, OrderStatus.PrintingShipped);
    }

    public async Task<OrderOperationResponse> CompletePrintingOrderAsync(int id)
    {
        // 先更新订单状态
        var result = await UpdateOrderStatusAsync(id, OrderStatus.PrintingCompleted);

        if (!result.Success)
        {
            return result;
        }

        // 获取订单信息以获取产品ID
        var order = await _context.PrintingOrders.FindAsync(id);
        if (order != null)
        {
            // 调用 InventoryService 更新库存
            try
            {
                await UpdateInventoryAfterPrintingAsync(order.ProductId);
            }
            catch (Exception ex)
            {
                // 记录错误但不影响订单状态更新
                Console.WriteLine($"更新库存失败: {ex.Message}");
            }
        }

        return result;
    }

    private async Task UpdateInventoryAfterPrintingAsync(int productId)
    {
        var httpClient = _httpClientFactory.CreateClient("InventoryService");
        var response = await httpClient.PostAsync($"/api/inventory/update-after-printing/{productId}", null);
        response.EnsureSuccessStatusCode();
    }

    private async Task<OrderOperationResponse> UpdateOrderStatusAsync(int id, string newStatus)
    {
        var order = await _context.PrintingOrders.FindAsync(id);

        if (order == null)
        {
            return new OrderOperationResponse
            {
                Success = false,
                Message = "印刷订单不存在"
            };
        }

        if (!StatusTransitionValidator.IsValidPrintingOrderTransition(order.Status, newStatus))
        {
            return new OrderOperationResponse
            {
                Success = false,
                Message = $"无法从状态 {order.Status} 转换到 {newStatus}",
                OrderNumber = order.OrderNumber,
                OrderId = order.Id,
                CurrentStatus = order.Status
            };
        }

        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new OrderOperationResponse
        {
            Success = true,
            Message = "印刷订单状态更新成功",
            OrderNumber = order.OrderNumber,
            OrderId = order.Id,
            CurrentStatus = order.Status
        };
    }

    private static PrintingOrderResponse MapToResponse(PrintingOrder order)
    {
        return new PrintingOrderResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            ApplicationOrderId = order.ApplicationOrderId,
            PrintingFactoryId = order.PrintingFactoryId,
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            Status = order.Status,
            Remarks = order.Remarks,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }
}
