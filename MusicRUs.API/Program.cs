using Microsoft.EntityFrameworkCore;
using MusicRUs.API.Data;
using MusicRUs.API.Backend.Logic.GoogleAuth;
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

// Google Auth
builder.Services.Configure<GoogleAuthOptions>(builder.Configuration.GetSection(GoogleAuthOptions.SectionName));
builder.Services.AddScoped<IGoogleAuthService, GoogleAuthService>();

// Repositories (Registering repositories for Dependency Injection)
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IReadRepository<>), typeof(Repository<>)); 
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IForumProfileRepository, ForumProfileRepository>();
builder.Services.AddScoped<IMusicCollectionRepository, MusicCollectionRepository>();

// Services (Registering services for Dependency Injection)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IForumProfileService, ForumProfileService>();
builder.Services.AddScoped<IMusicCollectionService, MusicCollectionService>();

// Database
var connectionString = builder.Configuration.GetConnectionString("MusicRUs")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:MusicRUs mangler i user-secrets eller miljøvariabler.");

builder.Services.AddSingleton<TimestampInterceptor>();
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
    options.UseNpgsql(connectionString)
           .UseSnakeCaseNamingConvention()
           .AddInterceptors(sp.GetRequiredService<TimestampInterceptor>()));

// CORS
const string FrontendCorsPolicy = "Frontend";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Development: allow any localhost/127.0.0.1-port
            policy.SetIsOriginAllowed(origin =>
                    Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
                    (uri.Host == "localhost" || uri.Host == "127.0.0.1"))
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        }
    });
});

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
app.UseCors(FrontendCorsPolicy);
app.UseAuthorization();
app.MapControllers();
app.Run();