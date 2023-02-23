using System;
using System.Text;
using System.Text.Json;
using Ganss.Excel;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace task3
{
	public class ExcelAPI
	{
		public string? HostName { get; set; } = "localhost";
		public int Port { get; set; } = 5672;
		public string? UserName { private get; set; } = Config.MQ_USERNAME;
		public string? Password { private get; set; } = Config.MQ_PASSWORD;
		public string? Prefix { get; set; } = "EXCEL";
		public string RequestQueueName { get; set; } = "rpc_excel";

		private ConnectionFactory? _factory;
		private IConnection? _connection;
		private IModel? _channel;

		private string? _consumerTag;
		private readonly string _excelFile = "/Users/lukasknudsen/Documents/dmu/sint/dmu_sint_classes/day07_message_construction/task3/Assets/ETA.xlsx";

		public void Connect()
		{
			_factory = new()
			{
				HostName = HostName,
				Port = Port,
				UserName = UserName,
				Password = Password,
			};
			_connection = _factory.CreateConnection();
			_channel = _connection.CreateModel();

			Console.WriteLine($" [{Prefix}] Connected to RabbitMQ on '{HostName}':'{Port}'");

			_channel.QueueDeclare(
				queue: RequestQueueName,
				durable: false,
				exclusive: false,
				autoDelete: false,
				arguments: null
			);

			Console.WriteLine($" [{Prefix}] Declared queue '{RequestQueueName}'");
		}

		public void StartListening()
		{
			if (_channel == null)
			{
				Console.WriteLine($" [{Prefix}] Not connected to RabbitMQ...");
				return;
			}

			Console.WriteLine($" [{Prefix}] Started listening for messages...");

			EventingBasicConsumer consumer = new(_channel);
			consumer.Received += (model, ea) =>
			{
				byte[] body = ea.Body.ToArray();
				string message = Encoding.UTF8.GetString(body);
				string routingKey = ea.RoutingKey;

				Console.WriteLine($" [{Prefix}] Received '{routingKey}':'{message}'");
				Messages.Arrival arrival = JsonSerializer.Deserialize<Messages.Arrival>(message);

				ExcelMapper excel = new(_excelFile);
				List<Messages.Arrival> arrivals = excel.Fetch<Messages.Arrival>().ToList();
				arrivals.Add(arrival);

				excel.Save(_excelFile, arrivals);

				Console.WriteLine($" [{Prefix}] Excel file updated!");
			};
			_consumerTag = _channel.BasicConsume(
				queue: RequestQueueName,
				autoAck: true,
				consumer: consumer
			);
		}

		public void StopListening()
		{
			if (_channel == null)
			{
				Console.WriteLine($" [{Prefix}] Not connected to RabbitMQ...");
				return;
			}

			_channel.BasicCancel(
				consumerTag: _consumerTag
			);

			Console.WriteLine($" [{Prefix}] Stopped listening for messages...");
		}
	}
}

