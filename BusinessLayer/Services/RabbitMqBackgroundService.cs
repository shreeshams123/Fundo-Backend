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
            rabbitMqService.StartConsuming("user_registration_queue", stoppingToken);
            await Task.CompletedTask;
        }
    }
}
