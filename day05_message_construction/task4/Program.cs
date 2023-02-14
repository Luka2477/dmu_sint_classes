namespace task4;
class Program
{
	static void Main(string[] args)
	{
		Airline sas = new()
		{
			Prefix = "SAS",
			RequestQueueName = "rpc_request_aic",
			ReplyQueueName = "rpc_reply_sas",
		};
		sas.Init((rkey, arrival) =>
		{
			Console.WriteLine(arrival);
		});

		Airline klm = new()
		{
			Prefix = "KLM",
			RequestQueueName = "rpc_request_aic",
			ReplyQueueName = "rpc_reply_klm",
		};
		klm.Init((rkey, arrival) =>
		{
			Console.WriteLine(arrival);
		});

		Airline sw = new()
		{
			Prefix = "SW",
			RequestQueueName = "rpc_request_aic",
			ReplyQueueName = "rpc_reply_sw",
		};
		sw.Init((rkey, arrival) =>
		{
			Console.WriteLine(arrival);
		});

		InformationControl aic = new()
		{
			RequestQueueName = "rpc_request_aic"
		};
		aic.Init(new List<Messages.Arrival> {
			new Messages.Arrival("SAS", "sas1234", "b12", new DateTime(2022, 02, 14, 12, 30, 00)),
			new Messages.Arrival("KLM", "klm1234", "b13", new DateTime(2022, 02, 14, 13, 30, 00)),
			new Messages.Arrival("SW", "sw1234", "b14", new DateTime(2022, 02, 14, 14, 30, 00)),
		});


		// -------------------------------------------------------------------------


		sas.RequestETA("sas1234");
		Thread.Sleep(1000);

		klm.RequestETA("klm1234");
		Thread.Sleep(1000);

		sw.RequestETA("sw1234");
		Thread.Sleep(1000);

		sw.RequestETA("ajdahshdad");

		Console.Read();
	}
}