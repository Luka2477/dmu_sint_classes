using System;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace task4
{
	public class RPCServer
	{
		public string? HostName { get; set; } = "localhost";
		public int Port { get; set; } = 5672;
		public string? UserName { private get; set; } = Config.MQ_USERNAME;
		public string? Password { private get; set; } = Config.MQ_PASSWORD;
		public string? RequestQueueName { get; set; }
		public string? Prefix { get; set; }

		private ConnectionFactory? _factory;
		private IConnection? _connection;
		private IModel? _channel;

		private string? _consumerTag;
		private List<Messages.Arrival> _arrivals;

		public RPCServer(List<Messages.Arrival> arrivals)
		{
			_arrivals = arrivals ?? throw new ArgumentNullException(nameof(arrivals));
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
				queue: RequestQueueName,
				durable: false,
				exclusive: false,
				autoDelete: false,
				arguments: null
			);

			Console.WriteLine($" [{Prefix}] Connected to RabbitMQ on '{HostName}':'{Port}'");
		}

		public void StartListening()
		{
			Console.WriteLine($" [{Prefix}] Started listening for messages...");

			EventingBasicConsumer consumer = new(_channel);
			consumer.Received += (model, ea) =>
			{
				IBasicProperties props = ea.BasicProperties;
				IBasicProperties replyProps = _channel.CreateBasicProperties();
				replyProps.CorrelationId = props.CorrelationId;

				byte[] body = ea.Body.ToArray();
				string message = Encoding.UTF8.GetString(body);
				string routingKey = ea.RoutingKey;

				Console.WriteLine($" [{Prefix}] Received '{routingKey}':'{message}'");

				string res = "INVALID";
				foreach (Messages.Arrival arrival in _arrivals)
				{
					if (arrival.FlightID.Equals(message))
					{
						res = JsonSerializer.Serialize(arrival);
					}
				}

				byte[] resBytes = Encoding.UTF8.GetBytes(res);
				_channel.BasicPublish(exchange: string.Empty,
														 routingKey: props.ReplyTo,
														 basicProperties: replyProps,
														 body: resBytes);
				_channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

				Console.WriteLine($" [{Prefix}] Sent '{props.ReplyTo}':'{res}'");
			};
			_consumerTag = _channel.BasicConsume(
				queue: RequestQueueName,
				autoAck: false,
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

