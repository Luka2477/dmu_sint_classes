using System;
using RabbitMQ.Client;
using System.Text;

namespace task6
{
	public class Sender
	{
		public string? HostName { get; set; }
		public int Port { get; set; }
		public string? UserName { private get; set; }
		public string? Password { private get; set; }
		public string? QueueName { get; set; }
		public string? ExchangeName { get; set; }
		public string? RoutingKey { get; set; }

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

			_channel.QueueDeclare(
				queue: QueueName,
				durable: false,
				exclusive: false,
				autoDelete: false,
				arguments: null
			);

			Console.WriteLine($" [*] Sender connected to RabbitMQ on {HostName}:{Port}");
		}

		public void CreateExchange()
		{
			if (_channel == null)
			{
				Console.WriteLine(" [*] Sender is not connected to RabbitMQ...");
				return;
			}

			_channel.ExchangeDeclare(
				exchange: ExchangeName,
				type: ExchangeType.Direct
			);

			_channel.QueueBind(
				queue: QueueName,
				exchange: ExchangeName,
				routingKey: RoutingKey ?? QueueName
			);

			Console.WriteLine(" [*] Sender exchange created and queue bound");
        }

        public void SendMessage(string message)
		{
			byte[] body = Encoding.UTF8.GetBytes(message);

			_channel.BasicPublish(
				exchange: ExchangeName ?? string.Empty,
				routingKey: RoutingKey ?? QueueName,
				basicProperties: null,
				body: body
			);

			Console.WriteLine($" [x] Sent {RoutingKey ?? QueueName}:{message}");
		}
	}
}

