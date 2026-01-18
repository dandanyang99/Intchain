using Intchain.Shared.Extensions;
using Intchain.InventoryService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add MySQL Database
builder.Services.AddMySqlDatabase<InventoryDbContext>(builder.Configuration);

// Add Redis Cache
builder.Services.AddRedisCache(builder.Configuration);

// Register custom services
builder.Services.AddScoped<Intchain.InventoryService.Services.Redis.IRedisLockService, Intchain.InventoryService.Services.Redis.RedisLockService>();
builder.Services.AddScoped<Intchain.InventoryService.Services.IProductService, Intchain.InventoryService.Services.ProductService>();
builder.Services.AddScoped<Intchain.InventoryService.Services.IInventoryService, Intchain.InventoryService.Services.InventoryService>();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Intchain Inventory Service API",
        Version = "v1",
        Description = "库存服务API - 负责彩票产品和库存管理"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory Service API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
