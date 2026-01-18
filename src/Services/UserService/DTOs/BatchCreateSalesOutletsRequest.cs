using System.ComponentModel.DataAnnotations;

namespace Intchain.UserService.DTOs;

/// <summary>
/// 批量创建销售网点请求
/// </summary>
public class BatchCreateSalesOutletsRequest
{
    /// <summary>
    /// 销售网点列表
    /// </summary>
    [Required(ErrorMessage = "销售网点列表不能为空")]
    [MinLength(1, ErrorMessage = "至少需要添加一个销售网点")]
    public List<CreateSalesOutletRequest> SalesOutlets { get; set; } = new();
}
