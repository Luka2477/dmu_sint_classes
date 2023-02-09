using System;
using RabbitMQ.Client;
using System.Text;

namespace task2
{
    public class Sender
    {
        public string Prefix { get; set; } = "*";
        public string HostName { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string? UserName { private get; set; }
        public string? Password { private get; set; }
        public string? QueueName { get; set; }
        public string? ExchangeName { get; set; }

        private ConnectionFactory? _factory;
        private IConnection? _connection;
        private IModel? _channel;

        public void Connect()
        {
            _factory = new()
            {
                HostName = HostName,
                Port = Port,
                UserName = UserName,
                Password = Password
            };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            Console.WriteLine($" [{Prefix}] Sender connected to RabbitMQ on {HostName}:{Port}");
        }

        public void CreateQueue()
        {
            if (_channel == null)
            {
                Console.WriteLine($" [{Prefix}] Sender is not connected to RabbitMQ...");
                return;
            }

            _channel.QueueDeclare(
                queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            Console.WriteLine($" [{Prefix}] Sender connected to queue {QueueName}");
        }

        public void CreateExchange()
        {
            if (_channel == null)
            {
                Console.WriteLine($" [{Prefix}] Sender is not connected to RabbitMQ...");
                return;
            }

            _channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: ExchangeType.Direct
            );

            Console.WriteLine($" [{Prefix}] Sender exchange created");
        }

        public void SendMessage(string message, string? routingKey = "")
        {
            byte[] body = Encoding.UTF8.GetBytes(message);

            routingKey = routingKey != null && routingKey != string.Empty ? routingKey : QueueName;

            _channel.BasicPublish(
                exchange: ExchangeName ?? string.Empty,
                routingKey: routingKey,
                basicProperties: null,
                body: body
            );

            Console.WriteLine($" [{Prefix}] Sent '{routingKey}':'{message}'");
        }
    }
}

