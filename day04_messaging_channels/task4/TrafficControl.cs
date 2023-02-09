using System;
namespace task4
{
	public class TrafficControl
	{
		private Sender _sender;

		public void CreateSender(string prefix = "*")
		{
			_sender = new()
			{
				Prefix = prefix,
				UserName = "guest",
				Password = "pass123!",
			};
			_sender.Connect();
		}

		public void Sender_CreateQueue(string queueName)
		{
			if (_sender == null)
			{
				throw new NullReferenceException("Sender must exist before creating a queue");
			}

			_sender.QueueName = queueName;
			_sender.CreateQueue();
		}

		public void Sender_CreateExchange(string exchangeName)
		{
			if (_sender == null)
			{
				throw new NullReferenceException("Sender must exist before creating an exchange");
			}

			_sender.ExchangeName = exchangeName;
			_sender.CreateExchange();
		}

		public void Sender_SendMessage(string message, string? routingKey = "")
		{
			_sender.SendMessage(message, routingKey);
		}
	}
}

