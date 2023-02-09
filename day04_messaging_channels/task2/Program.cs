using System.Text.Json;

namespace task2;

class Program
{
	static void Main(string[] args)
	{
		TrafficControl tc = new("Traffic Control", "TC");
		InformationCenter ic = new("Information Center", "IC");

		Messages.Arrival msgObj = new("SAS1234", "B12", new DateTime(2023, 02, 09, 12, 30, 00));
		string msgStr = JsonSerializer.Serialize(msgObj);

		ic.StartListening();
		tc.SendMessage(msgStr);

		Console.ReadLine();
	}
}

