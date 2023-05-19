using DemoApp.Contracts;
using DemoApp.Entities;
using DemoApp.Persistence;
using DemoApp.Routes;
using Microsoft.EntityFrameworkCore;
using Serilog;

var configuration = new LoggerConfiguration().MinimumLevel
    .Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console()
    .Enrich.FromLogContext();
var logger = configuration.CreateLogger();
Log.Logger = logger;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(logger);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>();

var app = builder.Build();

// Recreate the database
using var scope = app.Services.CreateScope();
AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await context.Database.EnsureDeletedAsync();
await context.Database.EnsureCreatedAsync();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapGroup("/users").MapUsersApiRoutes();
app.MapGroup("/posts").MapPostsApiRoutes();

app.Run();
