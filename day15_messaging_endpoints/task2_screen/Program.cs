using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace task2_screen;

class Program
{
    public static ICollection<string> Passengers { get; set; } = new List<string>();
    
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

        // Create screen queue
        string queueName = channel.QueueDeclare(
            queue: "day15_screen",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        ).QueueName;

        Console.WriteLine($"Created queue '{queueName}'");

        // Receive messages in checkin queue
        EventingBasicConsumer consumer = new(channel);
        consumer.Received += (ch, ea) =>
        {
            Console.WriteLine($"Received message from '{ea.RoutingKey}'");
            Console.WriteLine();

            // Get XML from byte array
            XElement passenger = XElement.Parse(Encoding.UTF8.GetString(ea.Body.ToArray()));
            string name = passenger.Element("Name")!.Value;
            string number = passenger.Element("ContactNumber")!.Value;
            
            // Add passenger to passengers list
            Passengers.Add($"{name} - +45{number}");

            Console.WriteLine("Here is the new passenger list:");
            PrintPassengers();
            Console.WriteLine();
        };

        // Attach consumer to checkin queue
        channel.BasicConsume(
            queue: queueName,
            autoAck: true,
            consumer: consumer
        );

        Console.WriteLine($"Listening to messages on '{queueName}'");
        Console.WriteLine();

        Console.WriteLine("Press 'ENTER' to exit...");
        Console.WriteLine();
        Console.Read();
    }

    private static void PrintPassengers()
    {
        foreach (string p in Passengers)
        {
            Console.WriteLine(p);
        }
    }
}