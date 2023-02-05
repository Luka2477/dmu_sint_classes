using System;
namespace task5
{
	public class Airline
	{
		public string Name { get; set; }

		public Airline(string name) => Name = name;

		public void ReceiveMessage()
		{
			Receiver receiver = new()
			{
				HostName = "localhost",
				Port = 5672,
				UserName = "guest",
				Password = "pass123!",
				QueueName = "TrafficControl"
			};

			receiver.Connect();
			receiver.WaitAndRead();
		}
	}
}

