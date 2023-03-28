using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;

namespace airline;

public class Airline
{
    public string Name;
    public Func<XElement> Ticket;
    public string QueueName;

    private bool _connected;
    private IModel _channel;

    private void Connect()
    {
        // Connect to RabbitMQ
        ConnectionFactory factory = new()
        {
            UserName = "guest",
            Password = "pass123!",
        };
        IConnection connection = factory.CreateConnection();
        _channel = connection.CreateModel();

        Console.WriteLine("Connected to RabbitMQ");

        // Create queue
        _channel.QueueDeclare(
            queue: $"day09_{QueueName}",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _connected = true;
    }

    public void SendTicket()
    {
        if (Ticket == null)
        {
            throw new Exception("Ticket must be defined");
        }

        if (!_connected)
        {
            Connect();
        }

        byte[] bytes = Encoding.UTF8.GetBytes(Ticket().ToString());

        _channel.BasicPublish(
            exchange: string.Empty,
            routingKey: $"day09_{QueueName}",
            body: bytes
        );

        Console.WriteLine($"[{Name,5}]   Ticket sent to 'day09_{QueueName}'");
        Console.WriteLine();
    }
}