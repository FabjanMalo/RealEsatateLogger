
using Confluent.Kafka;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using System.Text.Json;

namespace RealEsatateLogger;

public class EventConsumer : BackgroundService
{
    private readonly ILogger<EventConsumer> _logger;
    private readonly IServiceProvider _provider;
    public EventConsumer(ILogger<EventConsumer> logger, IServiceProvider provider)
    {
        _logger = logger;
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:29092",
            GroupId = "test-grup",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

        consumer.Subscribe("error-log");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(TimeSpan.FromSeconds(5));

                if (consumeResult is null) { continue; }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var errorLog = JsonSerializer.Deserialize<ErrorLog>(consumeResult.Message.Value, options);

                if (errorLog == null || string.IsNullOrEmpty(errorLog.Message))
                {
                    _logger.LogError("Received error log is null or the message is empty. Skipping.");
                    continue;
                }

                using (var scope = _provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<EstateLoggerDbContext>();


                    await context.ErrorLogs.AddAsync(errorLog, stoppingToken);
                    await context.SaveChangesAsync(stoppingToken);
                }

                _logger.LogInformation($"Consumed message {consumeResult.Message.Value} at: '{consumeResult.Offset}'");
            }
            catch (OperationCanceledException)
            {

                _logger.LogError("Error");
            }
        }


    }
}
