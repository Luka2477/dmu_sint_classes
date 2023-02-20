using System;
using System.Text.Json;

namespace task3
{
	public class Airline
	{
		public string? Prefix { get; set; } = "AIR";
		public string? RequestQueueName { get; set; }
		public string? ReplyQueueName { get; set; }

		private RPCClient? _rpcClient;

		public void Init(HandleMessageDelegate callback) 
		{
			_rpcClient = new(callback)
			{
				Prefix = Prefix,
				ReplyQueueName = ReplyQueueName,
			};
			_rpcClient.Connect();
			_rpcClient.CreateQueue();
			_rpcClient.StartListening();
		}

		public void RequestETA(string flightID)
		{
			if (_rpcClient == null)
			{
				throw new NullReferenceException("You must call 'Init()' first...");
			}

			_rpcClient.RequestQueueName = RequestQueueName;
			_rpcClient.SendMessage(flightID);
		}

		public void SendToExcel(Messages.Arrival arrival)
		{
			if (_rpcClient == null)
			{
				throw new NullReferenceException("You must call 'Init()' first...");
			}

			string message = JsonSerializer.Serialize(arrival);

			_rpcClient.RequestQueueName = "rpc_excel";
			_rpcClient.SendMessage(message);
		}
	}
}

