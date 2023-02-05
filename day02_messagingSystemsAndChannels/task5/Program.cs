namespace task5;
class Program
{
    static void Main(string[] args)
    {
        InformationCenter sas = new("Scandinavian Airline Service");
        Airline swa = new("South West Airlines");

        sas.SendMessage("south west airlines;12.00.00n13.00.00+01.00.00;SWA12345;copenhagen;checkin");

        Thread.Sleep(2000);

        swa.ReceiveMessage();
    }
}

