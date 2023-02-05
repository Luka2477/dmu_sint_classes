using System.Text;
using RabbitMQ.Client;

namespace task2;
class Program
{
	static void Main(string[] args)
	{
		const string HostName = "localhost";
		const int Port = 5672;
		const string UserName = "guest";
		const string Password = "pass123!";
		const string QueueName = "TestQueue";

        ConnectionFactory factory = new ()
        {
			HostName = HostName,
			Port = Port,
			UserName = UserName,
			Password = Password
		};
		IConnection connection = factory.CreateConnection();
		IModel channel = connection.CreateModel();

		channel.QueueDeclare(
			queue: QueueName,
			durable: false,
			exclusive: false,
			autoDelete: false,
			arguments: null
		);

		const string message = "Hello World!";
		byte[] body = Encoding.UTF8.GetBytes(message);

		channel.BasicPublish(
			exchange: string.Empty,
			routingKey: QueueName,
			basicProperties: null,
			body: body
		);

		Console.WriteLine($" [x] Sent {message}");

		Console.WriteLine(" Press [enter] to exit.");
		Console.ReadLine();
	}
}

