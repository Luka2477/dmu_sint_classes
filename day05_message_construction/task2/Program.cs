using RabbitMQ.Client;

namespace task2;
class Program
{
	public static void Main(string[] args)
	{
		RPCClient client = new((rkey, arrival) =>
		{
			Console.WriteLine("Callback called...");
		})
		{
			UserName = "guest",
			Password = "pass123!",
			Prefix = "CLIENT",
			RequestQueueName = "rpc_request",
			ReplyQueueName = "rpc_reply",
		};
		client.Connect();
		client.CreateQueue();
		client.StartListening();

		RPCServer server = new(new List<Messages.Arrival> {
			new Messages.Arrival("SAS", "SAS1234", "B12", new DateTime(2022, 02, 14, 12, 30, 00)),
			new Messages.Arrival("KLM", "KLM1234", "B13", new DateTime(2022, 02, 14, 13, 30, 00)),
			new Messages.Arrival("SW" , "SW1234" , "B14", new DateTime(2022, 02, 14, 14, 30, 00)),
		})
		{
			UserName = "guest",
			Password = "pass123!",
			Prefix = "SERVER",
			RequestQueueName = "rpc_request",
		};
		server.Connect();
		server.StartListening();

		// -------------------------------------------------------------------------

		client.SendMessage("SAS1234");
		Thread.Sleep(250);
		client.SendMessage("KLM1234");
		Thread.Sleep(250);
		client.SendMessage("SW0000");

		Console.ReadLine();
	}
}

