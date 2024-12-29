namespace BoTech.AvaloniaDesigner.Models.XML;

public class XmlBinding
{
    /// <summary>
    /// For example Binding; StaticResource...
    /// </summary>
    public string BindingType { get; set; } = string.Empty;
    /// <summary>
    /// The Text next to the Binding Type
    /// </summary>
    public string BindingValue { get; set; } = string.Empty;
}