using System;
namespace task4
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
				RequestQueueName = RequestQueueName,
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

			_rpcClient.SendMessage(flightID);
		}
	}
}

