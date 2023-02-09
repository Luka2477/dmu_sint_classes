using System;
namespace task3
{
	public class InformationCenter
	{
		private Receiver? _receiver;
		private Sender? _sender;

		public void CreateReceiver(string queueName, HandleMessageDelegate callback, string prefix = "*")
		{
			_receiver = new(callback)
			{
				Prefix = prefix,
				UserName = "guest",
				Password = "pass123!",
				QueueName = queueName,
			};
			_receiver.Connect();
		}

		public void Receiver_CreateExchange(string exchangeName, string routingKey)
		{
			if (_receiver == null)
			{
				throw new NullReferenceException("Receiver must exist before creating an exchange");
			}

			_receiver.ExchangeName = exchangeName;
			_receiver.RoutingKey = routingKey;
			_receiver.CreateExchangeAndBind();
		}

		public void Receiver_StartListening()
		{
			_receiver.StartListening();
		}

		public void Receiver_StopListening()
		{
			_receiver.StopListening();
		}

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

