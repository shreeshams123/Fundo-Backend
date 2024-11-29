using BusinessLayer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class RabbitMqBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public RabbitMqBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var rabbitMqService = scope.ServiceProvider.GetRequiredService<IRabbitMqService>();

            // Start consuming messages and pass the cancellation token
            rabbitMqService.StartConsuming("user_registration_queue", stoppingToken);

            // Wait for the service to be stopped
            await Task.CompletedTask;
        }
    }
}
