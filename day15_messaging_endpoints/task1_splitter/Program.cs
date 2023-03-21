using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace task1_splitter;
class Program
{
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
		channel.TxSelect();

		Console.WriteLine("Connected to RabbitMQ");

		// Create checkin queue
		string receiveQueueName = channel.QueueDeclare(
			queue: "day09_checkin",
			durable: false,
			exclusive: false,
			autoDelete: false,
			arguments: null
		).QueueName;

		Console.WriteLine($"Created queue '{receiveQueueName}'");

		// Create dummy queues
		string luggageQueueName = channel.QueueDeclare(
			queue: "day09_luggage",
			durable: false,
			exclusive: false,
			autoDelete: false,
			arguments: null
		).QueueName;

		Console.WriteLine($"Created dummy queue '{luggageQueueName}'");

		string passengerQueueName = channel.QueueDeclare(
			queue: "day09_passenger",
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
			Console.WriteLine();
			
			// Declare channel as transactional
			channel.TxSelect();

			Console.WriteLine("Began transaction");
			Console.WriteLine();

			try
			{
				// Generate CorrelationID
				IBasicProperties props = channel.CreateBasicProperties();
				props.CorrelationId = ea.BasicProperties.CorrelationId;

				// Get XML from byte array
				XElement xml = XElement.Parse(Encoding.UTF8.GetString(ea.Body.ToArray()));

				// Parse Luggage and Passenger elements
				IEnumerable<XElement> luggages = xml.Elements("Luggage");
				IEnumerable<XElement> passengers = xml.Elements("Passenger");

				// Send Luggage data to dummy queue
				List<XElement> luggagesList = luggages.ToList();
				foreach (XElement luggage in luggagesList)
				{
					// Add data to support sliding window principle
					int sequence = int.Parse(luggage.Element("Identification")?.Value);

					XElement data = new XElement("Data");
					data.Add(new XElement("Total", luggagesList.Count));
					data.Add(new XElement("Sequence", sequence));
					luggage.AddFirst(data);

					byte[] bytes = Encoding.UTF8.GetBytes(luggage.ToString());

					channel.BasicPublish(
						exchange: string.Empty,
						routingKey: luggageQueueName,
						basicProperties: props,
						body: bytes
					);

					Console.WriteLine($"Sent message to '{luggageQueueName}' with body:");
					Console.WriteLine(luggage.ToString());
					Console.WriteLine();
				}

				// Send Passenger data to dummy queue
				foreach (XElement passenger in passengers)
				{
					byte[] bytes = Encoding.UTF8.GetBytes(passenger.ToString());

					channel.BasicPublish(
						exchange: string.Empty,
						routingKey: passengerQueueName,
						basicProperties: props,
						body: bytes
					);

					Console.WriteLine($"Sent message to '{passengerQueueName}' with body:");
					Console.WriteLine(passenger.ToString());
					Console.WriteLine();
				}
				
				// Simulate a problem in the transaction
				// throw new Exception();
				
				// Commit transaction to tell RabbitMQ to write messages to disk
				channel.TxCommit();

				Console.WriteLine("Transaction committed");
				Console.WriteLine();
			}
			catch
			{
				// Experienced a problem in the transaction, so we must rollback messages
				channel.TxRollback();

				Console.WriteLine("Ran into a problem. Rolling back the transaction");
				Console.WriteLine();
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
		Console.WriteLine();
		Console.Read();
	}
}

