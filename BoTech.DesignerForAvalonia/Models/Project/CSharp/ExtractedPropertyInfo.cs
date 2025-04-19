
namespace BoTech.DesignerForAvalonia.Models.Project.CSharp;

public class ExtractedPropertyInfo
{
    /// <summary>
    /// The Name of the Property
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The XML Documentation
    /// </summary>
    public string Documentation { get; set; } = string.Empty;
    /// <summary>
    /// The Type with Namespace if necessary
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// The AccessModifier of the Property
    /// </summary>
    public Modifier AccessModifier { get; set; } = Modifier.Public;
    /// <summary>
    /// When the Property is defined with the keyword static
    /// </summary>
    public bool IsStatic { get; set; } = false;
    /// <summary>
    /// When the property is defined with the keyword constant
    /// </summary>
    public bool IsConstant { get; set; } = false;
    /// <summary>
    /// When the Property is defined with the keyword readonly
    /// </summary>
    public bool IsReadOnly { get; set; } = false;
    /// <summary>
    /// The definition of the default value. For Example <c>"new User("Florian");"</c>
    /// </summary>
    public string DefaultValue { get; set; } = string.Empty;
    /// <summary>
    /// When a Getter is defined in the brackets => <c>{get;}</c>
    /// </summary>
    public bool HasGetter { get; set; } = false;
    /// <summary>
    /// When a Setter is defined in the brackets => <c>{set;}</c>
    /// </summary>
    public bool HasSetter { get; set; } = false;

}