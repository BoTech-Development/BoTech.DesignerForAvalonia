using System.Reflection;
using System.Text.Json.Serialization;
using Avalonia.Controls;
using Avalonia.Data;

namespace BoTech.DesignerForAvalonia.Models.Project;

public class Binding
{
    /// <summary>
    /// The string for the Property in the Xml file => "{Binding MyProperty}"
    /// </summary>
    public required string XmlDefinition  { get; set; }
    /// <summary>
    /// The Property in the View Model
    /// </summary>
    [JsonIgnore]
    public PropertyInfo ViewModelProperty { get; set; }
    /// <summary>
    /// The Property which is defined in the selected control.
    /// </summary>
    [JsonIgnore]
    public PropertyInfo ControlPropertyInfo { get; set; }
    /// <summary>
    /// The Binding mode defines the "direction" of a Binding.
    /// </summary>
    public required BindingMode BindingMode { get; set; }
    [JsonIgnore]
    public Control ControlInstance { get; set; }
    [JsonIgnore]
    public object LastSourceValue { get; set; } = null;
    [JsonIgnore]
    public object LastReceiverValue { get; set; } = null;
    
}