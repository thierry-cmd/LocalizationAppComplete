using FluentValidation;
using localizationApp.API;
using localizationApp.API.Behaviors;
using localizationApp.API.Data;
using localizationApp.API.Mapping;
using localizationApp.API.Middleware;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.IO;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// App Options (for /info endpoint)
builder.Services.Configure<AppOptions>(
    builder.Configuration.GetSection(AppOptions.SectionName));


// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7288",
            "http://localhost:5288",
            "http://localhost:5000",
            "http://localhost:5001",
            "https://localizationapp-client.azurewebsites.net"  // Client Azure
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// Database
var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
var dbPath = isDocker ? "/app/data/api.db" : "api.db";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Health Checks
var hc = builder.Services.AddHealthChecks();

// Liveness: application process is running
hc.AddCheck(
    name: "self-live",
    check: () => HealthCheckResult.Healthy("Application is running"),
    tags: new[] { "live" }
);

// Readiness: application can serve requests (verify database connection)
hc.AddCheck(
    name: "database-ready",
    check: () =>
    {
        try
        {
            using var scope = builder.Services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.CanConnect();
            return HealthCheckResult.Healthy("Database connection is ready");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database connection failed", ex);
        }
    },
    tags: new[] { "ready" }
);


// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);


// Mapster configuration
MappingConfig.RegisterMappings();


var app = builder.Build();

// Create database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Middleware
app.UseMiddleware<ExceptionMiddleware>();

// Health Check endpoints
app.UseHealthChecks("/health", new HealthCheckOptions
{
    AllowCachingResponses = false,
    Predicate = r => r.Tags.Contains("live")
});

app.UseHealthChecks("/ready", new HealthCheckOptions
{
    AllowCachingResponses = false,
    Predicate = r => r.Tags.Contains("ready")
});



// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazor");
app.UseAuthorization();
app.MapControllers();

app.Run();