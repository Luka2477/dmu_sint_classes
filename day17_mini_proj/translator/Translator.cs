using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace translator;

public class Translator
{
    public string QueueName;
    public Func<XElement, XElement> Translate;

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

        // Create queues
        _channel.QueueDeclare(
            queue: $"day09_{QueueName}",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _channel.QueueDeclare(
            queue: "day09_shared_repo",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _connected = true;
    }

    public void Listen()
    {
        if (!_connected)
        {
            Connect();
        }

        // Receive messages in checkin queue
        EventingBasicConsumer consumer = new(_channel);
        consumer.Received += (ch, ea) =>
        {
            // Get XML from byte array
            XElement ticket = XElement.Parse(Encoding.UTF8.GetString(ea.Body.ToArray()));
            
            Console.WriteLine($"Received message from '{ea.RoutingKey}':");
            Console.WriteLine(ticket.ToString());
            Console.WriteLine();
            
            XElement translatedTicket = Translate(ticket);

            Console.WriteLine("Translated to:");
            Console.WriteLine(translatedTicket.ToString());
            Console.WriteLine();
            
            byte[] bytes = Encoding.UTF8.GetBytes(translatedTicket.ToString());

            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: "day09_shared_repo",
                body: bytes
            );
            
            Console.WriteLine("Translated ticket sent to 'day09_shared_repo'");
            Console.WriteLine();
        };

        // Attach consumer to checkin queue
        _channel.BasicConsume(
            queue: $"day09_{QueueName}",
            autoAck: true,
            consumer: consumer
        );
        
        Console.WriteLine($"Listening to messages on 'day09_{QueueName}'");
        Console.WriteLine();

        Console.WriteLine("Press 'ENTER' to exit...");
        Console.WriteLine();
        Console.Read();
    }
}