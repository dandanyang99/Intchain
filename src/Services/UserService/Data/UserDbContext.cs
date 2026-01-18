using Microsoft.EntityFrameworkCore;
using Intchain.UserService.Models;

namespace Intchain.UserService.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<LotteryCenter> LotteryCenters { get; set; }
    public DbSet<SalesOutlet> SalesOutlets { get; set; }
    public DbSet<PrintingFactory> PrintingFactories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置 SalesOutlet 与 LotteryCenter 的关系
        modelBuilder.Entity<SalesOutlet>()
            .HasOne(s => s.LotteryCenter)
            .WithMany()
            .HasForeignKey(s => s.LotteryCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        // 配置索引
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.OpenId);
    }
}
