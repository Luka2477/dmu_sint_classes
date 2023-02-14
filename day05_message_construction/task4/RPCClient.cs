using System;
using System.Data.Common;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace task4
{
	public delegate void HandleMessageDelegate(string rkey, Messages.Arrival arrival);

	public class RPCClient
	{
		public string? HostName { get; set; } = "localhost";
		public int Port { get; set; } = 5672;
		public string? UserName { private get; set; } = Config.MQ_USERNAME;
		public string? Password { private get; set; } = Config.MQ_PASSWORD;
		public string? Prefix { get; set; }
		public string? RequestQueueName { get; set; }
		public string? ReplyQueueName { get; set; }

		private ConnectionFactory? _factory;
		private IConnection? _connection;
		private IModel? _channel;

		private readonly HandleMessageDelegate _callback;
		private string? _consumerTag;
		private readonly List<string> _callbacks = new();

		public RPCClient(HandleMessageDelegate callback)
		{
			_callback = callback ?? throw new ArgumentNullException(nameof(callback));
		}

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

			Console.WriteLine($" [{Prefix}] Connected to RabbitMQ on '{HostName}':'{Port}'");
		}

		public void CreateQueue()
		{
			if (_channel == null)
			{
				Console.WriteLine($" [{Prefix}] Not connected to RabbitMQ...");
				return;
			}

			_channel.QueueDeclare(
				queue: ReplyQueueName,
				durable: false,
				exclusive: false,
				autoDelete: false,
				arguments: null
			);

			Console.WriteLine($" [{Prefix}] Declared queue '{ReplyQueueName}'");
		}

		public void SendMessage(string message)
		{
			string correlationId = Guid.NewGuid().ToString();
			IBasicProperties props = _channel.CreateBasicProperties();
			props.CorrelationId = correlationId;
			props.ReplyTo = ReplyQueueName;

			byte[] body = Encoding.UTF8.GetBytes(message);

			_channel.BasicPublish(
					exchange: string.Empty,
					routingKey: RequestQueueName,
					basicProperties: props,
					body: body
			);
			_callbacks.Add(correlationId);

			Console.WriteLine($" [{Prefix}] Sent '{RequestQueueName}':'{message}'");
		}

		public void StartListening()
		{
			Console.WriteLine($" [{Prefix}] Started listening for messages...");

			EventingBasicConsumer consumer = new(_channel);
			consumer.Received += (model, ea) =>
			{
				string correlationID = ea.BasicProperties.CorrelationId;

				if (!_callbacks.Contains(correlationID))
				{
					return;
				}

				_callbacks.Remove(correlationID);

				byte[] body = ea.Body.ToArray();
				string message = Encoding.UTF8.GetString(body);
				string routingKey = ea.RoutingKey;

				Console.WriteLine($" [{Prefix}] Received '{routingKey}':'{message}'");
				if (!message.Equals("INVALID"))
				{
					_callback(routingKey, JsonSerializer.Deserialize<Messages.Arrival>(message));
				}
			};
			_consumerTag = _channel.BasicConsume(
				queue: ReplyQueueName,
				autoAck: true,
				consumer: consumer
			);
		}

		public void StopListening()
		{
			_channel.BasicCancel(
				consumerTag: _consumerTag
			);

			Console.WriteLine($" [{Prefix}] Stopped listening for messages...");
		}
	}
}

