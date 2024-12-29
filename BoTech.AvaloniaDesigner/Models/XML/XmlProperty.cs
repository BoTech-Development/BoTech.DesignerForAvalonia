namespace BoTech.AvaloniaDesigner.Models.XML;

public class XmlProperty
{
    public string PropertyName { get; set; } = string.Empty;
    public string PropertyValue { get; set; } = string.Empty;
    public XmlBinding? Binding { get; set; } = null;
}