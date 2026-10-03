using System.Text.Json.Serialization;
namespace NIN;

public class BookDirectory: IElementMenu
{
    public string Label { get; set; } = string.Empty;
    public List<IElementMenu> Items {get;set;} = new();
    [JsonIgnore]
    public string Type { get; set; } = ElementConstants.TypeFolder;
    public string Glyph { get; set; } = ElementConstants.GlyphFolder;
}
