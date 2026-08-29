namespace NIN;

public class Book
{
    public string Cmd { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Glyph { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public bool RunInBackground { get; set; }
    public bool RunInTerminal { get; set; }
    public string Type { get; set; } = "bookmark";
}
