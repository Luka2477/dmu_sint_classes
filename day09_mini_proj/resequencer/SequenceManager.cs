using System.Xml.Linq;

namespace resequencer;

public class SequenceManager
{
    public int Next { get; set; } = 1;
    public Dictionary<int, XElement> Messages { get; } = new();
}