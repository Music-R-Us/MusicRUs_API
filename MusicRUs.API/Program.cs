using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Data;
using MusicRUs.API.Services;
using MusicRUs.API.Services.Interfaces;
using MusicRUs.API.Repositories;
using MusicRUs.API.Repositories.Interfaces;
using MusicRUs.API.Data.Interceptors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());


// Repositories (Registering repositories for Dependency Injection)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IReadRepository<>), typeof(Repository<>)); 
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services (Registering services for Dependency Injection)
builder.Services.AddScoped<IUserService, UserService>();

// Database
var connectionString = builder.Configuration.GetConnectionString("MusicRUs")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:MusicRUs mangler i user-secrets eller miljøvariabler.");

builder.Services.AddSingleton<TimestampInterceptor>();
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention()
           .AddInterceptors(sp.GetRequiredService<TimestampInterceptor>()));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();