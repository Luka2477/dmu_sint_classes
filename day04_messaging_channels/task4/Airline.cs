using System;
using System.Text.Json;

namespace task4
{
	public class Airline
	{
		private Receiver? _receiver;

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

		public static void HandleArrival(string rkey, string msg)
		{
			Messages.Arrival? msgObj = JsonSerializer.Deserialize<Messages.Arrival>(msg);
			Console.WriteLine(msgObj);
		}
	}
}

