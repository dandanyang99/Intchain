using Microsoft.EntityFrameworkCore;
using Intchain.InventoryService.Models;

namespace Intchain.InventoryService.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
    {
    }

    public DbSet<LotteryProduct> LotteryProducts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置索引
        modelBuilder.Entity<LotteryProduct>()
            .HasIndex(p => p.LotteryCenterId);

        modelBuilder.Entity<LotteryProduct>()
            .HasIndex(p => p.Name);
    }
}
