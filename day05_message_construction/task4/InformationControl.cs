using System;
namespace task4
{
	public class InformationControl
	{
		public string Prefix { get; set; } = "AIC";
		public string? RequestQueueName { get; set; }

		private RPCServer? _rpcServer;

		public void Init(List<Messages.Arrival> arrivals)
		{
			_rpcServer = new(arrivals)
			{
				Prefix = Prefix,
				RequestQueueName = RequestQueueName,
			};
			_rpcServer.Connect();
			_rpcServer.StartListening();
		}
	}
}

