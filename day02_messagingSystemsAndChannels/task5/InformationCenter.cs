using System;
namespace task5
{
	public class InformationCenter
	{
		public string Name { get; set; }

		public InformationCenter(string name) => Name = name;

		public void SendMessage(string message)
		{
			Sender sender = new()
			{
				HostName = "localhost",
				Port = 5672,
				UserName = "guest",
				Password = "pass123!",
				QueueName = "TrafficControl"
			};

			sender.Connect();
			sender.SendMessage(message);
		}
	}
}

