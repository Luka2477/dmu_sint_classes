namespace translator;

public class CDM
{
    public enum TimeOfDay
    {
        All = 1,
        Before = 2,
        Noon = 3,
        After = 4
    }

    public enum Passenger
    {
        Adult = 1,
        Youth = 2,
        Child = 3, Children = 3,
        Baby = 4,
        Unaccompanied = 5
    }

    public enum Category
    {
        Economy = 1,
        Premium = 2,
        Business = 3,
        First = 4
    }

    public enum TripType
    {
        One = 1,
        Round = 2,
        Multi = 3
    }

    public enum Month
    {
        Jan = 1, January = 1,
        Feb = 2, February = 2,
        Mar = 3, March = 3,
        Apr = 4, April = 4,
        May = 5,
        Jun = 6, June = 6,
        Jul = 7, July = 7,
        Aug = 8, August = 8,
        Sep = 9, September = 9,
        Oct = 10, October = 10,
        Nov = 11, November = 11,
        Dec = 12, December = 12
    }
}