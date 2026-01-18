using Intchain.Shared.Extensions;
using Intchain.LogisticsService.Data;
using Intchain.LogisticsService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add MySQL Database
builder.Services.AddMySqlDatabase<LogisticsDbContext>(builder.Configuration);

// Add Redis Cache
builder.Services.AddRedisCache(builder.Configuration);

// Register LogisticsService
builder.Services.AddScoped<ILogisticsService, LogisticsService>();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Intchain Logistics Service API",
        Version = "v1",
        Description = "物流服务API - 负责物流信息管理和第三方物流对接"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Logistics Service API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
