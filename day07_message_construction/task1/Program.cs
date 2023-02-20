using System.Text;
using RabbitMQ.Client;

namespace task1;
class Program
{
	static void Main(string[] args)
	{
		ConnectionFactory factory = new()
		{
			UserName = "guest",
			Password = "pass123!"
		};
		IConnection connection = factory.CreateConnection();
		IModel channel = connection.CreateModel();

		string queueName = channel.QueueDeclare(
			queue: "day07_task1",
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: null
		).QueueName;

		IBasicProperties properties = channel.CreateBasicProperties();
		properties.Expiration = "10000";

		byte[] message = Encoding.UTF8.GetBytes("Hello mother");
		channel.BasicPublish(
			exchange: string.Empty,
			routingKey: queueName,
			basicProperties: properties,
			body: message
		);
	}
}

