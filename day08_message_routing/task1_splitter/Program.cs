using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace task1_aic;
class Program
{
	private static readonly string receiveQueueName = "day08_task1_checkin";

	static void Main(string[] args)
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

		// Create dummy queues
		string luggageQueueName = channel.QueueDeclare(
			queue: "day08_task1_luggage",
			durable: false,
			exclusive: false,
			autoDelete: false,
			arguments: null
		).QueueName;

		Console.WriteLine($"Created dummy queue '{luggageQueueName}'");

		string passengerQueueName = channel.QueueDeclare(
			queue: "day08_task1_passenger",
			durable: false,
			exclusive: false,
			autoDelete: false,
			arguments: null
		).QueueName;

		Console.WriteLine($"Created dummy queue '{passengerQueueName}'");

		// Receive messages in checkin queue
		EventingBasicConsumer consumer = new(channel);
		consumer.Received += (ch, ea) =>
		{
			Console.WriteLine($"Received message from '{ea.RoutingKey}'");

			// Get XML from byte array
			XElement xml = XElement.Parse(Encoding.UTF8.GetString(ea.Body.ToArray()));

			// Parse Luggage and Passenger elements
			IEnumerable<XElement> luggages = xml.Elements("Luggage");
			IEnumerable<XElement> passengers = xml.Elements("Passenger");

			// Send Luggage data to dummy queue
			foreach (XElement luggage in luggages)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(luggage.ToString());

				channel.BasicPublish(
					exchange: string.Empty,
					routingKey: luggageQueueName,
					body: bytes
				);

				Console.WriteLine($"Sent message to '{luggageQueueName}' with body:");
				Console.WriteLine(luggage.ToString());
			}

			// Send Passenger data to dummy queue
			foreach (XElement passenger in passengers)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(passenger.ToString());

				channel.BasicPublish(
					exchange: string.Empty,
					routingKey: passengerQueueName,
					body: bytes
				);

				Console.WriteLine($"Sent message to '{passengerQueueName}' with body:");
				Console.WriteLine(passenger.ToString());
			}
		};

		// Attach consumer to checkin queue
		channel.BasicConsume(
			queue: receiveQueueName,
			autoAck: true,
			consumer: consumer
		);

		Console.WriteLine($"Listening to messages on '{receiveQueueName}'");

		Console.WriteLine();
		Console.WriteLine("Press 'ENTER' to exit...");
		Console.Read();
	}
}

