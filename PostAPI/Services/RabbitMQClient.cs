using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Text.Json;
using static Program;

namespace PostAPI.Services
{
    public class RabbitMQClient
    {
        private readonly RabbitMQSettings _settings;
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQClient(RabbitMQSettings settings)
        {
            _settings = settings;
            CreateConnection();
        }

        private void CreateConnection()
        {
            var factory = new ConnectionFactory() { HostName = _settings.Hostname };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void CreateConsumer(string queueName, Action<string> handleMessage)
        {
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                handleMessage(message);
            };
            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}


