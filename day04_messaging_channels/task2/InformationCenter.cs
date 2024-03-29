﻿using System;
namespace task2
{
	public class InformationCenter
	{
		private Receiver _receiver;

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

		public void CreateExchange(string exchangeName, string routingKey)
		{
			if (_receiver == null)
			{
				throw new NullReferenceException("Receiver must exist before creating an exchange");
			}

			_receiver.ExchangeName = exchangeName;
			_receiver.RoutingKey = routingKey;
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

