using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;



// Create connection to RabbitMQ
ConnectionFactory factory = new()
{
	UserName = "guest",
	Password = "pass123!",
};
IConnection connection = factory.CreateConnection();
IModel channel = connection.CreateModel();

Console.WriteLine("Connected to RabbitMQ");



// Create in queue
string CREADQueueName = channel.QueueDeclare(
	queue: "exam_c-read-out",
	durable: false,
	exclusive: false,
	autoDelete: false,
	arguments: null
).QueueName;
Console.WriteLine($"Created in queue '{CREADQueueName}'");



// Create out queues
string STASQueueName = channel.QueueDeclare(
	queue: "exam_stas-in",
	durable: false,
	exclusive: false,
	autoDelete: false,
	arguments: null
).QueueName;
Console.WriteLine($"Created out queue '{STASQueueName}'");

string SSQueueName = channel.QueueDeclare(
	queue: "exam_ss-in",
	durable: false,
	exclusive: false,
	autoDelete: false,
	arguments: null
).QueueName;
Console.WriteLine($"Created out queue '{SSQueueName}'");

string LMSQueueName = channel.QueueDeclare(
	queue: "exam_lms-in",
	durable: false,
	exclusive: false,
	autoDelete: false,
	arguments: null
).QueueName;
Console.WriteLine($"Created out queue '{LMSQueueName}'");

string PAYSQueueName = channel.QueueDeclare(
	queue: "exam_pays-in",
	durable: false,
	exclusive: false,
	autoDelete: false,
	arguments: null
).QueueName;
Console.WriteLine($"Created out queue '{PAYSQueueName}'");

string OutQueueName = channel.QueueDeclare(
	queue: "exam_out",
	durable: false,
	exclusive: false,
	autoDelete: false,
	arguments: null
).QueueName;
Console.WriteLine($"Created out queue '{OutQueueName}'");
Console.WriteLine();



// Define our consume which we will later use to subscribe to our in queue ('exam_c-reads-out')
EventingBasicConsumer consumer = new(channel);
consumer.Received += (ch, ea) =>
{
	Console.WriteLine($"Received message from '{ea.RoutingKey}'");
	Console.WriteLine();

	// Get XML from byte array
	XElement root = XElement.Parse(Encoding.UTF8.GetString(ea.Body.ToArray()));
	
	// Initialize routingKey to be the out queue ('exam_out')
	string routingKey = OutQueueName;
	// If the root elements name is 'AttendanceRegistration' or 'Payment'
	if (root.Name == "AttendanceRegistration")
	{
		// If payload contains 'Course' then it should be sent to LMS ('exam_lms-in')
		if (root.Element("Course") != null) 
			routingKey = LMSQueueName;
		// If payload contains 'Class' then it should be sent to SS ('exam_ss-in')
		else if (root.Element("Class") != null)
			routingKey = SSQueueName;
		// Else it should be sent to STAS ('exam_stas-in')
		else routingKey = STASQueueName;
	}
	// If the root elements name is 'Payment' it should be sent to ('exam_pays-in')
	else if (root.Name == "Payment")
		routingKey = PAYSQueueName;

	// Send the payload to the relevant queue via the routingKey
	channel.BasicPublish(
		exchange: string.Empty,
		routingKey: routingKey,
		body: ea.Body
	);

	Console.WriteLine($"Sent message to '{routingKey}' with body:");
	Console.WriteLine(ea.Body.ToString());
	Console.WriteLine();
};



// Subscribe to our in queue ('exam_c-reads-out')
channel.BasicConsume(
	queue: CREADQueueName,
	autoAck: true,
	consumer: consumer
);

Console.WriteLine($"Listening to messages on '{CREADQueueName}'");
Console.WriteLine();

Console.WriteLine("Press 'ENTER' to exit...");
Console.WriteLine();
Console.Read();