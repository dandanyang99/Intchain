using Microsoft.EntityFrameworkCore;
using Intchain.ApprovalService.Models;

namespace Intchain.ApprovalService.Data;

public class ApprovalDbContext : DbContext
{
    public ApprovalDbContext(DbContextOptions<ApprovalDbContext> options) : base(options)
    {
    }

    public DbSet<ApprovalRecord> ApprovalRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置索引
        modelBuilder.Entity<ApprovalRecord>()
            .HasIndex(a => a.ApplicationOrderId);

        modelBuilder.Entity<ApprovalRecord>()
            .HasIndex(a => a.ApproverId);
    }
}
