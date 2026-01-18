using Intchain.OrderService.Data;
using Intchain.OrderService.DTOs;
using Intchain.OrderService.Models;
using Intchain.OrderService.Utils;
using Intchain.OrderService.Constants;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Text.Json;

namespace Intchain.OrderService.Services;

/// <summary>
/// 申请订单服务实现
/// </summary>
public class ApplicationOrderService : IApplicationOrderService
{
    private readonly OrderDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOrderStatusHistoryService _historyService;

    public ApplicationOrderService(
        OrderDbContext context,
        IHttpClientFactory httpClientFactory,
        IOrderStatusHistoryService historyService)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _historyService = historyService;
    }

    public async Task<ApplicationOrderResponse?> CreateApplicationOrderAsync(CreateApplicationOrderRequest request)
    {
        // Generate order number first
        var orderNumber = OrderNumberGenerator.GenerateApplicationOrderNumber();

        // Reserve inventory before creating order
        var httpClient = _httpClientFactory.CreateClient("InventoryService");
        var reserveRequest = new
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            OrderId = orderNumber
        };

        var reserveResponse = await httpClient.PostAsJsonAsync("/api/inventory/reserve", reserveRequest);

        if (!reserveResponse.IsSuccessStatusCode)
        {
            // Inventory reservation failed
            var errorContent = await reserveResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"库存预留失败: {errorContent}");
        }

        // Create order after successful inventory reservation
        var order = new ApplicationOrder
        {
            OrderNumber = orderNumber,
            SalesOutletId = request.SalesOutletId,
            LotteryCenterId = request.LotteryCenterId,
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            Status = OrderStatus.ApplicationPending,
            Remarks = request.Remarks,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.ApplicationOrders.Add(order);
        await _context.SaveChangesAsync();

        return MapToResponse(order);
    }

    public async Task<ApplicationOrderResponse?> UpdateApplicationOrderAsync(int id, UpdateApplicationOrderRequest request)
    {
        var order = await _context.ApplicationOrders.FindAsync(id);

        if (order == null)
        {
            return null;
        }

        if (request.Quantity.HasValue)
        {
            order.Quantity = request.Quantity.Value;
        }

        if (request.Remarks != null)
        {
            order.Remarks = request.Remarks;
        }

        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponse(order);
    }

    public async Task<bool> DeleteApplicationOrderAsync(int id)
    {
        var order = await _context.ApplicationOrders.FindAsync(id);

        if (order == null)
        {
            return false;
        }

        _context.ApplicationOrders.Remove(order);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<ApplicationOrderResponse?> GetApplicationOrderAsync(int id)
    {
        var order = await _context.ApplicationOrders.FindAsync(id);

        return order == null ? null : MapToResponse(order);
    }

    public async Task<ApplicationOrderResponse?> GetApplicationOrderByNumberAsync(string orderNumber)
    {
        var order = await _context.ApplicationOrders
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        return order == null ? null : MapToResponse(order);
    }

    public async Task<List<ApplicationOrderResponse>> GetAllApplicationOrdersAsync()
    {
        var orders = await _context.ApplicationOrders.ToListAsync();

        return orders.Select(MapToResponse).ToList();
    }

    public async Task<List<ApplicationOrderResponse>> GetApplicationOrdersBySalesOutletAsync(int salesOutletId)
    {
        var orders = await _context.ApplicationOrders
            .Where(o => o.SalesOutletId == salesOutletId)
            .ToListAsync();

        return orders.Select(MapToResponse).ToList();
    }

    public async Task<List<ApplicationOrderResponse>> GetApplicationOrdersByLotteryCenterAsync(int lotteryCenterId)
    {
        var orders = await _context.ApplicationOrders
            .Where(o => o.LotteryCenterId == lotteryCenterId)
            .ToListAsync();

        return orders.Select(MapToResponse).ToList();
    }

    public async Task<List<ApplicationOrderResponse>> GetApplicationOrdersByStatusAsync(string status)
    {
        var orders = await _context.ApplicationOrders
            .Where(o => o.Status == status)
            .ToListAsync();

        return orders.Select(MapToResponse).ToList();
    }

    // Placeholder methods for Phase 3 implementation
    public async Task<OrderOperationResponse> ApproveApplicationOrderAsync(int id, ApproveApplicationOrderRequest request)
    {
        var order = await _context.ApplicationOrders.FindAsync(id);

        if (order == null)
        {
            return new OrderOperationResponse
            {
                Success = false,
                Message = "订单不存在"
            };
        }

        if (!StatusTransitionValidator.IsValidApplicationOrderTransition(order.Status, OrderStatus.ApplicationApproved))
        {
            return new OrderOperationResponse
            {
                Success = false,
                Message = $"无法从状态 {order.Status} 审批通过",
                OrderNumber = order.OrderNumber,
                OrderId = order.Id,
                CurrentStatus = order.Status
            };
        }

        // Confirm inventory deduction
        var httpClient = _httpClientFactory.CreateClient("InventoryService");
        var confirmRequest = new
        {
            ProductId = order.ProductId,
            Quantity = request.ApprovedQuantity,
            OrderId = order.OrderNumber
        };

        var confirmResponse = await httpClient.PostAsJsonAsync("/api/inventory/confirm", confirmRequest);

        if (!confirmResponse.IsSuccessStatusCode)
        {
            var errorContent = await confirmResponse.Content.ReadAsStringAsync();
            return new OrderOperationResponse
            {
                Success = false,
                Message = $"库存确认失败: {errorContent}",
                OrderNumber = order.OrderNumber,
                OrderId = order.Id,
                CurrentStatus = order.Status
            };
        }

        // TODO: Phase 4 - 创建印刷订单 (需要注入 IPrintingOrderService)
        // var printingOrderRequest = new CreatePrintingOrderRequest
        // {
        //     ApplicationOrderId = order.Id,
        //     PrintingFactoryId = request.PrintingFactoryId,
        //     ProductId = order.ProductId,
        //     Quantity = request.ApprovedQuantity,
        //     Remarks = request.ApprovalRemarks
        // };
        // await _printingOrderService.CreatePrintingOrderAsync(printingOrderRequest);

        // 保存原始状态用于历史记录
        var oldStatus = order.Status;

        order.Status = OrderStatus.ApplicationApproved;
        order.Quantity = request.ApprovedQuantity;
        order.Remarks = request.ApprovalRemarks ?? order.Remarks;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // 记录状态转换历史
        await _historyService.RecordStatusTransitionAsync(
            "Application",
            order.Id,
            oldStatus,
            order.Status,
            reason: request.ApprovalRemarks
        );

        return new OrderOperationResponse
        {
            Success = true,
            Message = "订单审批通过",
            OrderNumber = order.OrderNumber,
            OrderId = order.Id,
            CurrentStatus = order.Status
        };
    }

    public async Task<OrderOperationResponse> RejectApplicationOrderAsync(int id, RejectApplicationOrderRequest request)
    {
        var order = await _context.ApplicationOrders.FindAsync(id);

        if (order == null)
        {
            return new OrderOperationResponse
            {
                Success = false,
                Message = "订单不存在"
            };
        }

        if (!StatusTransitionValidator.IsValidApplicationOrderTransition(order.Status, OrderStatus.ApplicationRejected))
        {
            return new OrderOperationResponse
            {
                Success = false,
                Message = $"无法从状态 {order.Status} 拒绝",
                OrderNumber = order.OrderNumber,
                OrderId = order.Id,
                CurrentStatus = order.Status
            };
        }

        // Release reserved inventory
        var httpClient = _httpClientFactory.CreateClient("InventoryService");
        var releaseRequest = new
        {
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            OrderId = order.OrderNumber
        };

        var releaseResponse = await httpClient.PostAsJsonAsync("/api/inventory/release", releaseRequest);

        if (!releaseResponse.IsSuccessStatusCode)
        {
            var errorContent = await releaseResponse.Content.ReadAsStringAsync();
            return new OrderOperationResponse
            {
                Success = false,
                Message = $"库存释放失败: {errorContent}",
                OrderNumber = order.OrderNumber,
                OrderId = order.Id,
                CurrentStatus = order.Status
            };
        }

        // 保存原始状态用于历史记录
        var oldStatus = order.Status;

        order.Status = OrderStatus.ApplicationRejected;
        order.Remarks = request.RejectionReason ?? order.Remarks;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // 记录状态转换历史
        await _historyService.RecordStatusTransitionAsync(
            "Application",
            order.Id,
            oldStatus,
            order.Status,
            reason: request.RejectionReason
        );

        return new OrderOperationResponse
        {
            Success = true,
            Message = "订单已拒绝",
            OrderNumber = order.OrderNumber,
            OrderId = order.Id,
            CurrentStatus = order.Status
        };
    }

    public async Task<OrderOperationResponse> UpdateToWaitingShipmentAsync(int id)
    {
        return await UpdateOrderStatusAsync(id, OrderStatus.ApplicationWaitingShipment);
    }

    public async Task<OrderOperationResponse> UpdateToShippedAsync(int id)
    {
        return await UpdateOrderStatusAsync(id, OrderStatus.ApplicationShipped);
    }

    public async Task<OrderOperationResponse> UpdateToInTransitAsync(int id)
    {
        return await UpdateOrderStatusAsync(id, OrderStatus.ApplicationInTransit);
    }

    public async Task<OrderOperationResponse> CompleteApplicationOrderAsync(int id)
    {
        return await UpdateOrderStatusAsync(id, OrderStatus.ApplicationCompleted);
    }

    private async Task<OrderOperationResponse> UpdateOrderStatusAsync(int id, string newStatus)
    {
        var order = await _context.ApplicationOrders.FindAsync(id);

        if (order == null)
        {
            return new OrderOperationResponse
            {
                Success = false,
                Message = "订单不存在"
            };
        }

        if (!StatusTransitionValidator.IsValidApplicationOrderTransition(order.Status, newStatus))
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

        // 保存原始状态用于历史记录
        var oldStatus = order.Status;

        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // 记录状态转换历史
        await _historyService.RecordStatusTransitionAsync(
            "Application",
            order.Id,
            oldStatus,
            order.Status
        );

        return new OrderOperationResponse
        {
            Success = true,
            Message = "订单状态更新成功",
            OrderNumber = order.OrderNumber,
            OrderId = order.Id,
            CurrentStatus = order.Status
        };
    }

    private static ApplicationOrderResponse MapToResponse(ApplicationOrder order)
    {
        return new ApplicationOrderResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            SalesOutletId = order.SalesOutletId,
            LotteryCenterId = order.LotteryCenterId,
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            Status = order.Status,
            Remarks = order.Remarks,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }
}
