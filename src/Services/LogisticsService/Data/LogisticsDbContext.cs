using Microsoft.EntityFrameworkCore;
using Intchain.LogisticsService.Models;

namespace Intchain.LogisticsService.Data;

public class LogisticsDbContext : DbContext
{
    public LogisticsDbContext(DbContextOptions<LogisticsDbContext> options) : base(options)
    {
    }

    public DbSet<LogisticsInfo> LogisticsInfos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置索引
        modelBuilder.Entity<LogisticsInfo>()
            .HasIndex(l => l.TrackingNumber)
            .IsUnique();

        modelBuilder.Entity<LogisticsInfo>()
            .HasIndex(l => l.PrintingOrderId);
    }
}
