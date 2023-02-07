namespace task6;
class Program
{
    static void Main(string[] args)
    {
        TrafficControl tc = new("Traffic Information Control", "TC");
        Airline sas = new("Scandinavian Airline Service", "SAS");
        Airline klm = new("Royal Dutch Airlines", "KLM");
        Airline sw = new("South West Airlines", "SW");

        string[] routingKeys = { "sas", "klm", "sw" };

        sas.StartListening();
        klm.StartListening();
        sw.StartListening();

        while (true)
        {
            Thread.Sleep(3000);

            Random random = new();
            int i = random.Next(0, 3);

            tc.SendMessage("something", routingKeys[i]);
        }
    }
}

