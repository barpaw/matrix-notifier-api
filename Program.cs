using System.Text.Json.Serialization;
using MatrixNotifierApi.Commands;
using MediatR;
using Polly;
using MatrixNotifierApi.Commands;
using MatrixNotifierApi.Services;
using MatrixNotifierApi.Workers;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(theme: AnsiConsoleTheme.Code)
    .CreateLogger();

try
{
    Log.Information("Starting web application");


    var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Host.UseSerilog();
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
    
    builder.Services.AddMemoryCache();
    
    builder.Services.AddTransient<IApiService, ApiService>();
    builder.Services.AddSingleton<IConcurrentQueueService, ConcurrentQueueService>();
    
    builder.Services.AddHostedService<MatrixWorker>();
    
    var app = builder.Build();

// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }


    app.MapGet("/", () => { return Results.Ok("Working"); });

    app.MapGet("/health", () => { return Results.Ok(); });

    app.MapPost("/send-message",
        async (IMediator mediator, MatrixNotifier matrixNotifier) =>
        {
            var response = await mediator.Send(new WebhookCommand(matrixNotifier));

            return Results.Ok(response);
        });

    app.Run("http://localhost:8001");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}


public record MatrixNotifier(
    [property: JsonPropertyName("matrixHomeserverURL")]
    string MatrixHomeserverURL,
    [property: JsonPropertyName("matrixHomeserverUser")]
    string MatrixHomeserverUser,
    [property: JsonPropertyName("matrixHomeserverPasswd")]
    string MatrixHomeserverPasswd,
    [property: JsonPropertyName("matrixHomeserverRoom")]
    string MatrixHomeserverRoom,
    [property: JsonPropertyName("deviceId")]
    string DeviceId,
    [property: JsonPropertyName("message")]
    string Message
    
);