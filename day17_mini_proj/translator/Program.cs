using System.Xml.Linq;
using translator;

Translator swa = new()
{
    QueueName = "swa",
    Translate = (ticket) =>
    {
        XElement time = ticket.Element("TimeOfDay");
        XElement passenger = ticket.Element("Passenger");
        XElement category = ticket.Element("Category");
        XElement tripType = ticket.Element("TripType");
        XElement departure = ticket.Element("Departure");
        XElement arrival = ticket.Element("Arrival");

        string[] departureSplit = departure?.Value.Split("/");
        DateTime departureDate = new DateTime(DateTime.Now.Year, int.Parse(departureSplit[0]),
            int.Parse(departureSplit[1]));
        if ((departureDate - DateTime.Now).TotalSeconds < 0)
        {
            departureDate = new DateTime(DateTime.Now.Year + 1, int.Parse(departureSplit[0]),
                int.Parse(departureSplit[1]));
        }

        time?.SetValue(Enum.Parse(typeof(CDM.TimeOfDay), time.Value.Split(" ")[0]));
        passenger?.SetValue(Enum.Parse(typeof(CDM.Passenger),
            passenger.Value.Split(" ")[0].Equals("Teens") ? "Youth" : passenger.Value.Split(" ")[0]));
        category?.SetValue(Enum.Parse(typeof(CDM.Category), category.Value.Split(" ")[0]));
        tripType?.SetValue(Enum.Parse(typeof(CDM.TripType), tripType.Value.Split(" ")[0]));
        departure?.SetValue(departureDate.ToLongDateString());

        if (arrival != null)
        {
            string[] arrivalSplit = arrival?.Value.Split("/");
            DateTime arrivalDate = new DateTime(DateTime.Now.Year, int.Parse(arrivalSplit[0]),
                int.Parse(arrivalSplit[1]));
            if ((arrivalDate - DateTime.Now).TotalSeconds < 0)
            {
                arrivalDate = new DateTime(DateTime.Now.Year + 1, int.Parse(arrivalSplit[0]),
                    int.Parse(arrivalSplit[1]));
            }

            arrival?.SetValue(arrivalDate.ToLongDateString());
        }


        return ticket;
    }
};

Translator klm = new()
{
    QueueName = "klm",
    Translate = (ticket) =>
    {
        XElement passenger = ticket.Element("Passenger");
        XElement category = ticket.Element("Category");
        XElement tripType = ticket.Element("TripType");
        XElement departure = ticket.Element("Departure");
        XElement arrival = ticket.Element("Arrival");

        string[] departureSplit = departure?.Value.Split(" ");
        DateTime departureDate = new DateTime(DateTime.Now.Year,
            (int)Enum.Parse(typeof(CDM.Month), departureSplit[1]),
            int.Parse(departureSplit[0]));
        if ((departureDate - DateTime.Now).TotalSeconds < 0)
        {
            departureDate = departureDate.AddYears(1);
        }

        passenger?.SetValue(Enum.Parse(typeof(CDM.Passenger),
            passenger.Value.Split(" ")[0].Equals("Student") ? "Adult" : passenger.Value.Split(" ")[0]));
        category?.SetValue(Enum.Parse(typeof(CDM.Category),
            category.Value.Split(" ")[0].Equals("La") ? "First" : category.Value.Split(" ")[0]));
        tripType?.SetValue(Enum.Parse(typeof(CDM.TripType), tripType.Value.Split(" ")[0]));
        departure?.SetValue(departureDate.ToLongDateString());

        if (arrival != null)
        {
            string[] arrivalSplit = arrival?.Value.Split(" ");
            DateTime arrivalDate = new DateTime(DateTime.Now.Year,
                (int)Enum.Parse(typeof(CDM.Month), arrivalSplit[1]),
                int.Parse(arrivalSplit[0]));
            if ((arrivalDate - DateTime.Now).TotalSeconds < 0)
            {
                arrivalDate = arrivalDate.AddYears(1);
            }

            arrival?.SetValue(arrivalDate.ToLongDateString());
        }

        return ticket;
    }
};

Translator sas = new()
{
    QueueName = "sas",
    Translate = (ticket) =>
    {
        XElement passenger = ticket.Element("Passenger");
        XElement category = ticket.Element("Category");
        XElement tripType = ticket.Element("TripType");
        XElement departure = ticket.Element("Departure");
        XElement arrival = ticket.Element("Arrival");

        string[] departureSplit = departure?.Value.Split(" ");
        DateTime departureDate = new DateTime(DateTime.Now.Year,
            (int)Enum.Parse(typeof(CDM.Month), departureSplit[2]),
            int.Parse(departureSplit[1]));
        if ((departureDate - DateTime.Now).TotalSeconds < 0)
        {
            departureDate = departureDate.AddYears(1);
        }

        passenger?.SetValue(Enum.Parse(typeof(CDM.Passenger),
            passenger.Value.Split(" ")[0]));
        category?.SetValue(Enum.Parse(typeof(CDM.Category), category.Value.Split(" ")[0]));
        tripType?.SetValue(Enum.Parse(typeof(CDM.TripType), tripType.Value.Split(" ")[0]));
        departure?.SetValue(departureDate.ToLongDateString());

        if (arrival != null)
        {
            string[] arrivalSplit = arrival?.Value.Split(" ");
            DateTime arrivalDate = new DateTime(DateTime.Now.Year,
                (int)Enum.Parse(typeof(CDM.Month), arrivalSplit[2]),
                int.Parse(arrivalSplit[1]));
            if ((arrivalDate - DateTime.Now).TotalSeconds < 0)
            {
                arrivalDate = arrivalDate.AddYears(1);
            }

            arrival?.SetValue(arrivalDate.ToLongDateString());
        }

        return ticket;
    }
};

bool running = true;
while (running)
{
    Console.WriteLine("Please enter what you would like to do:");
    Console.WriteLine("--- 'tran <airline>' to translate incoming tickets");
    Console.WriteLine("------- Airlines: 'swa', 'klm', 'sas'");
    Console.WriteLine("--- Leave empty to quit");
    string input = Console.ReadLine() ?? "";
    string[] split = input.Split(" ");

    Console.WriteLine();
    switch (split[0])
    {
        case "tran":
            switch (split[1])
            {
                case "swa":
                    swa.Listen();
                    break;
                case "klm":
                    klm.Listen();
                    break;
                case "sas":
                    sas.Listen();
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