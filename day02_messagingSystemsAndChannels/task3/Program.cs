using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace task3;
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

		Console.WriteLine(" [*] Waiting for messages.");

        EventingBasicConsumer consumer = new (channel);
		consumer.Received += (model, ea) =>
		{
			byte[] body = ea.Body.ToArray();
			string message = Encoding.UTF8.GetString(body);
			Console.WriteLine($" [x] Received {message}");
		};
		channel.BasicConsume(
			queue: QueueName,
			autoAck: true,
			consumer: consumer
		);

		Console.WriteLine(" Press [enter] to exit.");
		Console.ReadLine();
	}
}

