using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;

namespace task2_sender;

static class Program
{
    public static void Main(string[] args)
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
            queue: "day15_screen",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        ).QueueName;

        Console.WriteLine($"Created queue '{queueName}'");
        Console.WriteLine();

        while (true)
        {
            Console.WriteLine("Type 'send' to send message...");
            Console.WriteLine("Leave empty to exit...");
            string input = Console.ReadLine();

            switch (input)
            {
                case "send":
                    Console.WriteLine("How many messages would you like to send?");
                    int number = int.Parse(Console.ReadLine());

                    for (int i = 0; i < number; i++)
                    {
                        SendMessage(channel, queueName, RandomPassenger());
                    }

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

    private static void SendMessage(IModel channel, string queueName, XElement xml)
    {
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
    }

    private static XElement RandomPassenger()
    {
        XElement p = new("Passenger");
        p.Add(new XElement("Name"));
        p.Element("Name")!.Value = RandomName(6, 18);
        p.Add(new XElement("ContactNumber"));
        p.Element("ContactNumber")!.Value = RandomNumber(8);

        return p;

        static string RandomName(int minl, int maxl)
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyz ";
            Random rng = new Random();
            string name = rng.NextStrings(allowedChars, (minl, maxl));
            return char.ToUpper(name[0]) + name[1..];
        }

        static string RandomNumber(int l)
        {
            const string allowedChars = "0123456789";
            Random rng = new Random();
            return rng.NextStrings(allowedChars, (l, l));
        }
    }

    static string NextStrings(this Random rnd, string allowedChars, (int Min, int Max) length)
    {
        (int min, int max) = length;
        char[] chars = new char[max];
        int setLength = allowedChars.Length;
        int stringLength = rnd.Next(min, max + 1);
        for (int i = 0; i < stringLength; ++i)
        {
            chars[i] = allowedChars[rnd.Next(setLength)];
        }

        return new string(chars, 0, stringLength);
    }
}