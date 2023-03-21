using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;

namespace task1_sender;
class Program
{
	private const string PathToXml = "/Users/lukasknudsen/Documents/dmu/sint/dmu_sint_classes/day09_mini_proj/task1_sender/Assets/Data.xml";

	private static void Main(string[] args)
	{
		// Connect to RabbitMQ
		ConnectionFactory factory = new()
		{
			UserName = "guest",
			Password = "pass123!",
		};
		IConnection connection = factory.CreateConnection();
		IModel channel = connection.CreateModel();

		Console.WriteLine("Connected to RabbitMQ");

		// Create queue
		string queueName = channel.QueueDeclare(
			queue: "day09_checkin",
			durable: false,
			exclusive: false,
			autoDelete: false,
			arguments: null
		).QueueName;

		Console.WriteLine($"Created queue '{queueName}'");
		Console.WriteLine();

		// Main loop
		while (true)
		{
			Console.WriteLine("Type 'send' to send message...");
			Console.WriteLine("Leave empty to exit...");
			string input = Console.ReadLine();

			switch (input)
			{
				case "send":
					Console.WriteLine();
					SendMessage(channel, queueName);
					break;
				case "":
					return;
				default:
					Console.WriteLine();
					Console.WriteLine("Unknown command...");
					Console.WriteLine();
					break;
			}
		}
	}
	

	private static void SendMessage(IModel channel, string queueName)
	{
		// Load XML from file
		XElement xml = XElement.Load(PathToXml);

		Console.WriteLine("Loaded XML file");
		
		// Generate CorrelationID
		IBasicProperties props = channel.CreateBasicProperties();
		props.CorrelationId = Guid.NewGuid().ToString();

		// Convert XML to byte array
		byte[] bytes = Encoding.UTF8.GetBytes(xml.ToString());

		Console.WriteLine("Converted XML to byte array");
		Console.WriteLine();

		// Publish message to queue
		channel.BasicPublish(
			exchange: string.Empty,
			routingKey: queueName,
			basicProperties: props,
			body: bytes
		);

		Console.WriteLine($"Sent message to '{queueName}' with body:");
		Console.WriteLine(xml.ToString());
		Console.WriteLine();
	}
}

