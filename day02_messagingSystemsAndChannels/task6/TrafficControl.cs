using System;
namespace task6
{
	public class TrafficControl
	{
		public string Name { get; set; }

		private Sender _sender;

		public TrafficControl(string name, string prefix = "*")
		{
			Name = name;

			_sender = new()
			{
				Prefix = prefix,
				UserName = "guest",
				Password = "pass123!",
				ExchangeName = "direct_tc",
			};
			_sender.Connect();
			_sender.CreateExchange();
		}

		public void SendMessage(string message, string routingKey)
		{
			_sender.SendMessage(message, routingKey);
		}
    }
}

