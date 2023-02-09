using System;
namespace task2
{
	public class InformationCenter
	{
		public string Name { get; set; }

		private readonly Receiver _receiver;

		public InformationCenter(string name, string prefix = "*")
		{
			Name = name;

			_receiver = new()
			{
				Prefix = prefix,
				UserName = "guest",
				Password = "pass123!",
				QueueName = "arrivals",
			};
			_receiver.Connect();
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

