using System.Text;
using System.Xml.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace resequencer;

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

        Console.WriteLine("Connected to RabbitMQ");

        // Create aggregator queue
        string aggregatorQueueName = channel.QueueDeclare(
            queue: "day09_aggregator",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        ).QueueName;

        Console.WriteLine($"Created queue '{aggregatorQueueName}'");

        // Create dummy queues
        string luggageQueueName = channel.QueueDeclare(
            queue: "day09_luggage",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        ).QueueName;

        Console.WriteLine($"Created dummy queue '{luggageQueueName}'");

        // Define buffer dictionary<correlationID, tuple<desiredSequence, dictionary<sequence, xElement>>>
        Dictionary<string, SequenceManager> buffer = new();

        // Receive messages in luggage queue
        EventingBasicConsumer consumer = new(channel);
        consumer.Received += (ch, ea) =>
        {
            Console.WriteLine($"Received message from '{ea.RoutingKey}'");

            IBasicProperties props = ea.BasicProperties;

            // Parse Luggage and Data elements
            XElement luggage = XElement.Parse(Encoding.UTF8.GetString(ea.Body.ToArray()));
            XElement data = luggage.Element("Data");
            int total = int.Parse(data.Element("Total").Value);
            int sequence = int.Parse(data.Element("Sequence").Value);

            Console.WriteLine($"Message '{sequence}' out of '{total}'");

            // Create sequence manager if doesn't exist for correlationID
            if (!buffer.ContainsKey(props.CorrelationId))
            {
                buffer.Add(props.CorrelationId, new SequenceManager());
                Console.WriteLine($"Created sequence manager with id '{props.CorrelationId}'");
            }

            // Add message to sequence manager
            buffer[props.CorrelationId].Messages.Add(sequence, luggage);
            Console.WriteLine($"Added message '{sequence}' to sequence manager with id '{props.CorrelationId}'");
            Console.WriteLine();

            // Check if sequence manager has next message in sequence
            // If it does, then send the luggage
            bool hasNextMessage = buffer[props.CorrelationId].Messages.ContainsKey(buffer[props.CorrelationId].Next);
            Console.WriteLine($"Sequence manager has next message '{hasNextMessage}'");
            while (hasNextMessage)
            {
                SendLuggage(
                    channel: channel,
                    queueName: aggregatorQueueName,
                    xElement: luggage,
                    ea: ea
                );
                Console.WriteLine($"Sent message to '{aggregatorQueueName}' with sequence number '{sequence}'");
                Console.WriteLine();

                buffer[props.CorrelationId].Next++;

                hasNextMessage = buffer[props.CorrelationId].Messages.ContainsKey(buffer[props.CorrelationId].Next);
                Console.WriteLine($"Sequence manager has next message '{hasNextMessage}'");
            }

            Console.WriteLine();

            // Remove sequence manager if all messages have been sent
            if (buffer[props.CorrelationId].Next > total)
            {
                buffer.Remove(props.CorrelationId);
                Console.WriteLine($"Removed sequence manager with id '{props.CorrelationId}'");
                Console.WriteLine();
            }
        };

        // Attach consumer to checkin queue
        channel.BasicConsume(
            queue: luggageQueueName,
            autoAck: false,
            consumer: consumer
        );

        Console.WriteLine($"Listening to messages on '{luggageQueueName}'");
        Console.WriteLine();

        Console.WriteLine("Press 'ENTER' to exit...");
        Console.WriteLine();
        Console.Read();
    }

    private static void SendLuggage(IModel channel, string queueName, XElement xElement, BasicDeliverEventArgs ea)
    {
        // Add CorrelationID to props
        IBasicProperties props = channel.CreateBasicProperties();
        props.CorrelationId = ea.BasicProperties.CorrelationId;

        // Send luggage to queue
        byte[] bytes = Encoding.UTF8.GetBytes(xElement.ToString());

        channel.BasicPublish(
            exchange: string.Empty,
            routingKey: queueName,
            basicProperties: props,
            body: bytes
        );

        // Acknowledge message in receiving queue
        channel.BasicAck(
            deliveryTag: ea.DeliveryTag,
            multiple: false
        );
    }
}