using Microsoft.EntityFrameworkCore;

namespace Intchain.ApprovalService.Data;

public class ApprovalDbContext : DbContext
{
    public ApprovalDbContext(DbContextOptions<ApprovalDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // 实体配置将在后续添加
    }
}
