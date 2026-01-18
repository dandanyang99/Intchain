using Microsoft.EntityFrameworkCore;
using Intchain.OrderService.Models;

namespace Intchain.OrderService.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<ApplicationOrder> ApplicationOrders { get; set; }
    public DbSet<PrintingOrder> PrintingOrders { get; set; }
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置索引
        modelBuilder.Entity<ApplicationOrder>()
            .HasIndex(a => a.OrderNumber)
            .IsUnique();

        modelBuilder.Entity<PrintingOrder>()
            .HasIndex(p => p.OrderNumber)
            .IsUnique();

        modelBuilder.Entity<PrintingOrder>()
            .HasIndex(p => p.ApplicationOrderId);

        // 配置OrderStatusHistory索引
        modelBuilder.Entity<OrderStatusHistory>()
            .HasIndex(h => new { h.OrderType, h.OrderId });

        modelBuilder.Entity<OrderStatusHistory>()
            .HasIndex(h => h.CreatedAt);
    }
}
