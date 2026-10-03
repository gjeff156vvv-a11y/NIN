namespace NIN;

public class Direcrory: Element
{
    public List<Element> Items {get;set;} = new();
    public string Type { get; set; } = ElementConstants.TypeFolder;
    public string Glyph { get; set; } = ElementConstants.GlyphFolder;
}
