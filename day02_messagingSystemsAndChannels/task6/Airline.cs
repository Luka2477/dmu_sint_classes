using System;
namespace task6
{
	public class Airline
	{
		public string Name { get; set; }

		private readonly Receiver _receiver;

		public Airline(string name, string prefix = "*")
		{
			Name = name;

			_receiver = new()
			{
				Prefix = prefix,
				UserName = "guest",
				Password = "pass123!",
				QueueName = prefix.ToLower(),
				ExchangeName = "direct_tc",
				RoutingKey = prefix.ToLower()
            };
			_receiver.Connect();
			_receiver.CreateExchangeAndBind();
		}

		public void StartListening()
		{
			_receiver.StartListening();
		}

		public void StopListening()
		{
			_receiver.StopListening();
		}
	}
}

