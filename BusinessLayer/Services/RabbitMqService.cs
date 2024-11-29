using BusinessLayer.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Models.DTOs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;



namespace BusinessLayer.Services
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly IConnection _connection;
        private readonly RabbitMQ.Client.IModel _channel;
        private readonly IEmailService _emailService;
        private const string UserRegistrationQueue = "user_registration_queue";
        private readonly ILogger<RabbitMqService> _logger;
        public RabbitMqService(ConnectionFactory connectionFactory,IEmailService emailService)
        {
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _emailService = emailService;

        }
        public void SendMessage(string message)
        {
            _channel.QueueDeclare(queue: UserRegistrationQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: UserRegistrationQueue, basicProperties: null, body: body);
        }

        public void StartConsuming(string queueName, CancellationToken cancellationToken)
        {
            _channel.QueueDeclare(queue:UserRegistrationQueue,durable:false,exclusive:false,autoDelete:false,arguments:null);
            var consumer=new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                try
                {
                    var emailMessage = JsonSerializer.Deserialize<EmailMessageDto>(message);
                    
                    if (emailMessage != null)
                    {
                        await _emailService.SendMailAsync(emailMessage);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from queue");
                }
            };
            _channel.BasicConsume(queue:UserRegistrationQueue,autoAck:true,consumer:consumer);
            cancellationToken.Register(() => _channel.Close());
        }

    }
}