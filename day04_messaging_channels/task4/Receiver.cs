using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace task4
{
	public delegate void HandleMessageDelegate(string rkey, string msg);

	public class Receiver
	{
		public string Prefix { get; set; } = "*";
		public string HostName { get; set; } = "localhost";
		public int Port { get; set; } = 5672;
		public string? UserName { private get; set; }
		public string? Password { private get; set; }
		public string? QueueName { get; set; }
		public string? ExchangeName { get; set; }
		public string? RoutingKey { get; set; }

		private HandleMessageDelegate _callback;
		private ConnectionFactory? _factory;
		private IConnection? _connection;
		private IModel? _channel;
		private bool _interrupt = false;
		private string? _consumerTag;

		public Receiver(HandleMessageDelegate callback)
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

			_channel.QueueDeclare(
				queue: QueueName,
				durable: false,
				exclusive: false,
				autoDelete: false,
				arguments: null
			);

			Console.WriteLine($" [{Prefix}] Connected to RabbitMQ on {HostName}:{Port}");
		}

		public void CreateExchangeAndBind()
		{
			if (_channel == null)
			{
				Console.WriteLine($" [{Prefix}] Receiver is not connected to RabbitMQ...");
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

			Console.WriteLine($" [{Prefix}] Receiver exchange created and queue bound");
		}

		public void ReadMessage()
		{
			BasicGetResult result = _channel.BasicGet(
				queue: QueueName,
				autoAck: true
			);

			if (result == null)
			{
				Console.WriteLine($" [{Prefix}] No messages to read...");
			}
			else
			{
				byte[] body = result.Body.ToArray();
				string message = Encoding.UTF8.GetString(body);
				string routingKey = result.RoutingKey;

				Console.WriteLine($" [{Prefix}] Received {routingKey}:{message}");
				_callback(routingKey, message);
			}
		}

		public void WaitAndRead()
		{
			Console.WriteLine($" [{Prefix}] Waiting for message...");

			_interrupt = false;
			BasicGetResult? result = null;
			while (result == null && _interrupt == false)
			{
				result = _channel.BasicGet(
					queue: QueueName,
					autoAck: true
				);
			}

			byte[] body = result.Body.ToArray();
			string message = Encoding.UTF8.GetString(body);
			string routingKey = result.RoutingKey;

			Console.WriteLine($" [{Prefix}] Received {routingKey}:{message}");
			_callback(routingKey, message);
		}

		public void Interrupt()
		{
			Console.WriteLine($" [{Prefix}] Interrupting waiting for message...");

			_interrupt = true;
		}

		public void StartListening()
		{
			Console.WriteLine($" [{Prefix}] Started listening for messages...");

			EventingBasicConsumer consumer = new(_channel);
			consumer.Received += (model, ea) =>
			{
				byte[] body = ea.Body.ToArray();
				string message = Encoding.UTF8.GetString(body);
				string routingKey = ea.RoutingKey;

				Console.WriteLine($" [{Prefix}] Received {routingKey}:{message}");
				_callback(routingKey, message);
			};
			_consumerTag = _channel.BasicConsume(
				queue: QueueName,
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

