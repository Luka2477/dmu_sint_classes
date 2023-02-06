using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace task6
{
	public class Receiver
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
		private bool _interrupt = false;
		private string? _consumerTag;

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

			Console.WriteLine($" [*] Connected to RabbitMQ on {HostName}:{Port}");
		}

		public void CreateExchangeAndBind()
		{
			if (_channel == null)
			{
				Console.WriteLine(" [*] Receiver is not connected to RabbitMQ...");
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

			Console.WriteLine(" [*] Receiver exchange created and queue bound");
		}

		public void ReadMessage()
		{
			BasicGetResult result = _channel.BasicGet(
				queue: QueueName,
				autoAck: true
			);

			if (result == null)
			{
				Console.WriteLine($" [*] No messages to read...");
			}
			else
			{
				byte[] body = result.Body.ToArray();
				string message = Encoding.UTF8.GetString(body);
				string routingKey = result.RoutingKey;
				Console.WriteLine($" [x] Received {routingKey}:{message}");
			}
		}

		public void WaitAndRead()
		{
			Console.WriteLine($" [*] Waiting for message...");

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
			Console.WriteLine($" [x] Received {routingKey}:{message}");
		}

		public void Interrupt()
		{
			Console.WriteLine(" [*] Interrupting waiting for message...");

			_interrupt = true;
		}

		public void StartListening()
		{
			Console.WriteLine(" [*] Started listening for messages...");

			EventingBasicConsumer consumer = new(_channel);
			consumer.Received += (model, ea) =>
			{
				byte[] body = ea.Body.ToArray();
				string message = Encoding.UTF8.GetString(body);
				string routingKey = ea.RoutingKey;

                Console.WriteLine($" [x] Received {routingKey}:{message}");
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

			Console.WriteLine(" [*] Stopped listening for messages...");
		}
	}
}

