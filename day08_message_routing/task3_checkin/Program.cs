using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;

namespace task3_checkin;
class Program
{
	private static readonly string _pathToXML = "/Users/lukasknudsen/Documents/dmu/sint/dmu_sint_classes/day08_message_routing/task3_checkin/Assets/Data.xml";

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
			queue: "day08_task3_checkin",
			durable: false,
			exclusive: false,
			autoDelete: false,
			arguments: null
		).QueueName;

		Console.WriteLine($"Created queue '{queueName}'");
		Console.WriteLine();

		// Load XML from file
		XElement xml = XElement.Load(_pathToXML);

		Console.WriteLine("Loaded XML file");

		// Convert XML to byte array
		byte[] bytes = Encoding.UTF8.GetBytes(xml.ToString());

		Console.WriteLine("Converted XML to byte array");
		Console.WriteLine();

		// Publish message to queue
		channel.BasicPublish(
			exchange: string.Empty,
			routingKey: queueName,
			body: bytes
		);

		Console.WriteLine($"Sent message to '{queueName}' with body:");
		Console.WriteLine(xml.ToString());
		Console.WriteLine();

		Console.WriteLine("Press 'ENTER' to exit...");
		Console.Read();
	}
}

