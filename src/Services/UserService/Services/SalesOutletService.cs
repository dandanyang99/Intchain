using Intchain.UserService.Data;
using Intchain.UserService.DTOs;
using Intchain.UserService.Models;
using Microsoft.EntityFrameworkCore;

namespace Intchain.UserService.Services;

/// <summary>
/// 销售网点服务实现
/// </summary>
public class SalesOutletService : ISalesOutletService
{
    private readonly UserDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public SalesOutletService(UserDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<SalesOutletResponse> CreateSalesOutletAsync(CreateSalesOutletRequest request)
    {
        // 验证彩票中心是否存在
        var lotteryCenterExists = await _context.LotteryCenters
            .AnyAsync(lc => lc.Id == request.LotteryCenterId);

        if (!lotteryCenterExists)
        {
            throw new InvalidOperationException($"彩票中心ID {request.LotteryCenterId} 不存在");
        }

        // 创建销售网点
        var salesOutlet = new SalesOutlet
        {
            Name = request.Name,
            LotteryCenterId = request.LotteryCenterId,
            ContactPerson = request.ContactPerson,
            ContactPhone = request.ContactPhone,
            Address = request.Address,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.SalesOutlets.Add(salesOutlet);
        await _context.SaveChangesAsync();

        // 生成默认用户名和密码
        var defaultUsername = request.DefaultUsername ?? GenerateDefaultUsername(salesOutlet.Id);
        var defaultPassword = request.DefaultPassword ?? GenerateDefaultPassword();

        // 创建默认用户
        var defaultUser = new User
        {
            Username = defaultUsername,
            PasswordHash = _passwordHasher.HashPassword(defaultPassword),
            Role = "SalesOutlet",
            EntityId = salesOutlet.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Users.Add(defaultUser);
        await _context.SaveChangesAsync();

        // 返回响应
        return new SalesOutletResponse
        {
            Id = salesOutlet.Id,
            Name = salesOutlet.Name,
            LotteryCenterId = salesOutlet.LotteryCenterId,
            ContactPerson = salesOutlet.ContactPerson,
            ContactPhone = salesOutlet.ContactPhone,
            Address = salesOutlet.Address,
            IsActive = salesOutlet.IsActive,
            CreatedAt = salesOutlet.CreatedAt,
            UpdatedAt = salesOutlet.UpdatedAt,
            DefaultUser = new DefaultUserInfo
            {
                UserId = defaultUser.Id,
                Username = defaultUsername,
                Password = defaultPassword // 返回明文密码供用户记录
            }
        };
    }

    public async Task<List<SalesOutletResponse>> BatchCreateSalesOutletsAsync(BatchCreateSalesOutletsRequest request)
    {
        var responses = new List<SalesOutletResponse>();

        foreach (var salesOutletRequest in request.SalesOutlets)
        {
            var response = await CreateSalesOutletAsync(salesOutletRequest);
            responses.Add(response);
        }

        return responses;
    }

    public async Task<SalesOutletResponse?> GetSalesOutletAsync(int id)
    {
        var salesOutlet = await _context.SalesOutlets.FindAsync(id);

        if (salesOutlet == null)
        {
            return null;
        }

        return MapToResponse(salesOutlet);
    }

    public async Task<List<SalesOutletResponse>> GetAllSalesOutletsAsync()
    {
        var salesOutlets = await _context.SalesOutlets.ToListAsync();
        return salesOutlets.Select(MapToResponse).ToList();
    }

    public async Task<List<SalesOutletResponse>> GetSalesOutletsByLotteryCenterIdAsync(int lotteryCenterId)
    {
        var salesOutlets = await _context.SalesOutlets
            .Where(so => so.LotteryCenterId == lotteryCenterId)
            .ToListAsync();

        return salesOutlets.Select(MapToResponse).ToList();
    }

    public async Task<SalesOutletResponse> UpdateSalesOutletAsync(int id, UpdateSalesOutletRequest request)
    {
        var salesOutlet = await _context.SalesOutlets.FindAsync(id);

        if (salesOutlet == null)
        {
            throw new InvalidOperationException($"销售网点ID {id} 不存在");
        }

        // 更新字段
        if (!string.IsNullOrEmpty(request.Name))
        {
            salesOutlet.Name = request.Name;
        }

        if (!string.IsNullOrEmpty(request.ContactPerson))
        {
            salesOutlet.ContactPerson = request.ContactPerson;
        }

        if (!string.IsNullOrEmpty(request.ContactPhone))
        {
            salesOutlet.ContactPhone = request.ContactPhone;
        }

        if (!string.IsNullOrEmpty(request.Address))
        {
            salesOutlet.Address = request.Address;
        }

        if (request.IsActive.HasValue)
        {
            salesOutlet.IsActive = request.IsActive.Value;
        }

        salesOutlet.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToResponse(salesOutlet);
    }

    public async Task<bool> DeleteSalesOutletAsync(int id)
    {
        var salesOutlet = await _context.SalesOutlets.FindAsync(id);

        if (salesOutlet == null)
        {
            return false;
        }

        _context.SalesOutlets.Remove(salesOutlet);
        await _context.SaveChangesAsync();

        return true;
    }

    private static SalesOutletResponse MapToResponse(SalesOutlet salesOutlet)
    {
        return new SalesOutletResponse
        {
            Id = salesOutlet.Id,
            Name = salesOutlet.Name,
            LotteryCenterId = salesOutlet.LotteryCenterId,
            ContactPerson = salesOutlet.ContactPerson,
            ContactPhone = salesOutlet.ContactPhone,
            Address = salesOutlet.Address,
            IsActive = salesOutlet.IsActive,
            CreatedAt = salesOutlet.CreatedAt,
            UpdatedAt = salesOutlet.UpdatedAt
        };
    }

    private static string GenerateDefaultUsername(int salesOutletId)
    {
        return $"outlet_{salesOutletId}";
    }

    private static string GenerateDefaultPassword()
    {
        // 生成8位随机密码
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
