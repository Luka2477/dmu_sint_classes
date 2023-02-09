using System;
namespace task2
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
				QueueName = "arrivals",
			};
			_sender.Connect();
			_sender.CreateQueue();
		}

		public void SendMessage(string message)
		{
			_sender.SendMessage(message);
		}
    }
}

