using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace UdemyRabbitMQWeb.Watermark.Services
{
    public class RabbitMQClientService : IDisposable
    {
        private readonly RabbitMQ.Client.ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "ImageDirectExchange";
        public static string RoutingWatermark = "watermark-route-image";
        public static string QueueName = "queue-watermark-image";

        private readonly ILogger<RabbitMQClientService> logger;

        public RabbitMQClientService(RabbitMQ.Client.ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> _logger)
        {
            _connectionFactory = connectionFactory;
            logger = _logger;
        }
        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection();
            if (_channel is { IsOpen: true })
            {
                return _channel;
            }
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct, true, false);
            _channel.QueueDeclare(QueueName, true, false, false, null);
            _channel.QueueBind(QueueName, ExchangeName, RoutingWatermark);
            logger.LogInformation("RabbitMQ ile bağlantı kuruldu.");
            return _channel;

        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            logger.LogInformation("RabbitMQ ile bağlantı kapatıldı.");
        }
    }
}
