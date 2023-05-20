using System.Text.Json.Serialization;
using DemoApp.Persistence;
using DemoApp.Routes;
using DemoApp.Services;
using Serilog;

var configuration = new LoggerConfiguration().MinimumLevel
    .Debug()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override(
        "Microsoft.EntityFrameworkCore",
        Serilog.Events.LogEventLevel.Information
    )
    .MinimumLevel.Override(
        "Microsoft.EntityFrameworkCore.Database.Command",
        Serilog.Events.LogEventLevel.Information
    )
    .WriteTo.Console()
    .Enrich.FromLogContext();
var logger = configuration.CreateLogger();
Log.Logger = logger;
var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(logger));
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(logger);
var DateTimeService = new DateTimeService();
builder.Services.AddSingleton<IDateTimeService, DateTimeService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();
builder.Services
    .AddControllers()
    .AddNewtonsoftJson(
        x =>
            x.SerializerSettings.ReferenceLoopHandling = Newtonsoft
                .Json
                .ReferenceLoopHandling
                .Ignore
    );
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<AppDbContext>();
var app = builder.Build();

using var scope = app.Services.CreateScope();
AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//await context.Database.EnsureDeletedAsync();
await context.Database.EnsureCreatedAsync();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapGroup("/users").MapUsersApiRoutes();
app.MapGroup("/posts").MapPostsApiRoutes();

app.Run();
