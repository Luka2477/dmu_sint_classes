using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;


// Connect to RabbitMQ
ConnectionFactory factory = new()
{
    UserName = "guest",
    Password = "pass123!",
};
IConnection connection = factory.CreateConnection();
IModel channel = connection.CreateModel();

Console.WriteLine("Connected to RabbitMQ");



// Create out queue
string queueName = channel.QueueDeclare(
    queue: "exam_c-read-out",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null
).QueueName;

Console.WriteLine($"Created queue '{queueName}'");
Console.WriteLine();



// Load XML from file
XElement root =
    XElement.Load(
        "/Users/lukasknudsen/Documents/dmu/sint/dmu_sint_classes/eksamensprojekt/code/simulated_sender/Assets/data.xml");
Console.WriteLine("Loaded XML file");



// Loop asking for what message you would like to send
bool shouldRun = true;
while (shouldRun)
{
    Console.Write("What message would you like to send? [STAS, SS, LMS, PAYS, OUT] ");
    string input = Console.ReadLine().ToLower();

    switch (input)
    {
        case "stas":
            SendMessage(root.Element("STAS").Element("AttendanceRegistration"));
            break;
        case "ss":
            SendMessage(root.Element("SS").Element("AttendanceRegistration"));
            break;
        case "lms":
            SendMessage(root.Element("LMS").Element("AttendanceRegistration"));
            break;
        case "pays":
            SendMessage(root.Element("PAYS").Element("Payment"));
            break;
        case "out":
            SendMessage(root.Element("Out").Element("Response"));
            break;
        case "":
            shouldRun = false;
            break;
        default:
            Console.WriteLine("Unknown command. Please try again...");
            Console.WriteLine();
            break;
    }
}



// Method for sending messages into the out queue
void SendMessage(XElement message)
{
    // Convert XML to byte array
    byte[] bytes = Encoding.UTF8.GetBytes(message.ToString());

    Console.WriteLine("Converted XML to byte array");
    Console.WriteLine();

    // Publish message to queue
    channel.BasicPublish(
        exchange: string.Empty,
        routingKey: queueName,
        body: bytes
    );
    
    Console.WriteLine($"Sent message to '{queueName}' with body:");
    Console.WriteLine(message.ToString());
    Console.WriteLine();
}