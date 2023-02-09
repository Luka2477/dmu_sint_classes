using System.Text.Json;

namespace task3;

class Program
{
	static void Main(string[] args)
	{
		TrafficControl tc = new();
		tc.CreateSender("TC");
		tc.Sender_CreateQueue("arrivals");

		InformationCenter ic = new();
		ic.CreateSender("IC");
		ic.Sender_CreateExchange("direct_arrivals");
		ic.CreateReceiver(
			queueName: "arrivals",
			prefix: "IC",
			callback: (rkey, msg) =>
			{
				ic.Sender_SendMessage(msg, "arrivals");
			}
		);
		ic.Receiver_StartListening();

		Airline klm = new();
		klm.CreateReceiver(
			queueName: "klm_arrivals",
			prefix: "KLM",
			callback: Airline.HandleArrival
		);
		klm.Receiver_CreateExchange("direct_arrivals", "arrivals");
		klm.Receiver_StartListening();

		Airline sas = new();
		sas.CreateReceiver(
			queueName: "sas_arrivals",
			prefix: "SAS",
			callback: Airline.HandleArrival
		);
		sas.Receiver_CreateExchange("direct_arrivals", "arrivals");
		sas.Receiver_StartListening();

		Airline sw = new();
		sw.CreateReceiver(
			queueName: "sw_arrivals",
			prefix: "SW",
			callback: Airline.HandleArrival
		);
		sw.Receiver_CreateExchange("direct_arrivals", "arrivals");
		sw.Receiver_StartListening();

		Messages.Arrival msgObj = new("SAS1234", "B12", new DateTime(2023, 02, 09, 12, 30, 00));
		string msgStr = JsonSerializer.Serialize(msgObj);

		tc.Sender_SendMessage(msgStr);

		Console.ReadLine();
	}
}

