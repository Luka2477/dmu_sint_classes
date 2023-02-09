using System.Text.Json;

namespace task2;

class Program
{
	static void Main(string[] args)
	{
		TrafficControl tc = new();
		InformationCenter ic = new();

		tc.CreateSender("TC");
		tc.CreateQueue("arrivals");

		ic.CreateReceiver(
			queueName: "arrivals",
			prefix: "IC",
			callback: (rkey, msg) =>
			{
				Messages.Arrival? msgObj = JsonSerializer.Deserialize<Messages.Arrival>(msg);
				Console.WriteLine(msgObj);
			}
		);
		ic.StartListening();

		Messages.Arrival msgObj = new("SAS1234", "B12", new DateTime(2023, 02, 09, 12, 30, 00));
		string msgStr = JsonSerializer.Serialize(msgObj);

		tc.SendMessage(msgStr);

		Console.ReadLine();
	}
}

