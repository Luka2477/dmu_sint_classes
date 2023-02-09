using System;
namespace task3
{
	public class Messages
	{
		public class Arrival
		{
			public string FlightID { get; private set; }
			public string Gate { get; private set; }
			public DateTime ETA { get; set; }

			public Arrival(string flightID, string gate, DateTime eTA)
			{
				FlightID = flightID ?? throw new ArgumentNullException(nameof(flightID));
				Gate = gate ?? throw new ArgumentNullException(nameof(gate));
				ETA = eTA;
			}

			public override string ToString()
			{
				return $"At {ETA} {FlightID} will arrive at {Gate}";
			}
		}
	}
}

