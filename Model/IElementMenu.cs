using System.Text.Json.Serialization;
namespace NIN;

[JsonDerivedType(typeof(Book), "bookmark")]
[JsonDerivedType(typeof(BookDirectory), "folder")]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization, IgnoreUnrecognizedTypeDiscriminators = false)]
public interface IElementMenu
{
    public string Label { get; set; }
    public string Type { get; set; }
}
