namespace NIN;

public class Book: Element
{
    public string Cmd { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool RunInBackground { get; set; }
    public bool RunInTerminal { get; set; }
    public string Type { get; set; } = "bookmark";
}
