using RabbitMQ.Client.Events;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PostAPI.Models;
using PostAPI.Services;
using static Program;
using PostAPI.Models.DTOs;
using Microsoft.Extensions.DependencyInjection;

namespace PostAPI.Services
{
    public class RabbitMQConsumerService : BackgroundService
    {
        private readonly RabbitMQSettings _settings;
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public RabbitMQConsumerService(IOptions<RabbitMQSettings> options, IServiceProvider serviceProvider)
        {
            _settings = options.Value;
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory() { HostName = _settings.Hostname };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: _settings.QueueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonConvert.DeserializeObject<NewImageMessage>(content);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var postService = scope.ServiceProvider.GetRequiredService<IPostService>();
                    await CreatePostAsync(message, postService);
                }

                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume(_settings.QueueName, false, consumer);

            await Task.CompletedTask;
        }

        private async Task CreatePostAsync(NewImageMessage message, IPostService postService)
        {
            var post = new Post
            {
                Title = message.Title,
                Description = message.Description,
                ImageFileName = message.ImageFileName,
                FileExtension = message.FileExtension
            };

            await postService.CreatePostAsync(post);
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}