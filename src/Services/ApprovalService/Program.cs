using Intchain.Shared.Extensions;
using Intchain.ApprovalService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add MySQL Database
builder.Services.AddMySqlDatabase<ApprovalDbContext>(builder.Configuration);

// Add Redis Cache
builder.Services.AddRedisCache(builder.Configuration);

// Register custom services
builder.Services.AddScoped<Intchain.ApprovalService.Services.IApprovalService, Intchain.ApprovalService.Services.ApprovalService>();

// Add HTTP client for OrderService (future integration)
builder.Services.AddHttpClient("OrderService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5003");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Intchain Approval Service API",
        Version = "v1",
        Description = "审批服务API - 负责审批流程管理和审批记录"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Approval Service API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
