using System.Xml.Linq;
using airline;

Airline swa = new()
{
    Name = "SWA",
    QueueName = "swa",
    Ticket = () =>
    {
        XElement timeOfDay = new XElement("TimeOfDay");
        timeOfDay.Add("Before noon");
        XElement type = new XElement("TripType");
        type.Add("One way");
        XElement passenger = new XElement("Passenger");
        passenger.Add("Adult Age 18+");
        XElement category = new XElement("Category");
        category.Add("Economy");
        XElement departure = new XElement("Departure");
        departure.Add("3/31");
        
        XElement ticket = new XElement("Ticket");
        ticket.Add(timeOfDay, type, passenger, category, departure);
        return ticket;
    }
};

Airline klm = new()
{
    Name = "KLM",
    QueueName = "klm",
    Ticket = () =>
    {
        XElement type = new XElement("TripType");
        type.Add("Round trip");
        XElement passenger = new XElement("Passenger");
        passenger.Add("Student (18-28 years)");
        XElement category = new XElement("Category");
        category.Add("Premium Comfort Class");
        XElement departure = new XElement("Departure");
        departure.Add("28 March");
        XElement arrival = new XElement("Arrival");
        arrival.Add("2 April");
        
        XElement ticket = new XElement("Ticket");
        ticket.Add(type, passenger, category, departure, arrival);
        return ticket;
    }
};

Airline sas = new()
{
    Name = "SAS",
    QueueName = "sas",
    Ticket = () =>
    {
        XElement type = new XElement("TripType");
        type.Add("Multi city");
        XElement passenger = new XElement("Passenger");
        passenger.Add("Children (2-11 years)");
        XElement category = new XElement("Category");
        category.Add("First Class");
        XElement departure = new XElement("Departure");
        departure.Add("Tue 28 March");
        XElement arrival = new XElement("Arrival");
        arrival.Add("Wed 5 Apr");
        
        XElement ticket = new XElement("Ticket");
        ticket.Add(type, passenger, category, departure, arrival);
        return ticket;
    }
};

bool running = true;
while (running)
{
    Console.WriteLine("Please enter what you would like to do:");
    Console.WriteLine("--- 'send <airline>' to send a ticket");
    Console.WriteLine("------- Airlines: 'swa', 'klm', 'sas'");
    Console.WriteLine("--- Leave empty to quit");
    string input = Console.ReadLine() ?? "";
    string[] split = input.Split(" ");

    Console.WriteLine();
    switch (split[0])
    {
        case "send":
            switch (split[1])
            {
                case "swa":
                    swa.SendTicket();
                    break;
                case "klm":
                    klm.SendTicket();
                    break;
                case "sas":
                    sas.SendTicket();
                    break;
                default:
                    Console.WriteLine("Unknown airline...");
                    break;
            }

            break;
        case "":
            running = false;
            break;
        default:
            Console.WriteLine("Unknown command...");
            break;
    }

    Console.WriteLine();
}