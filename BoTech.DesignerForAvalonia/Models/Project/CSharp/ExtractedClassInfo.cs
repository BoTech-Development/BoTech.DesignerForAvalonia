using System.Collections.Generic;

namespace BoTech.DesignerForAvalonia.Models.Project.CSharp;

public class ExtractedClassInfo
{
    /// <summary>
    /// The Classname of this Class
    /// </summary>
    public string ClassName { get; set; }
    /// <summary>
    /// The xml Documentation
    /// </summary>
    public string Documentation { get; set; } = string.Empty;
    /// <summary>
    /// The full namespace of this Class.
    /// </summary>
    public string Namespace { get; set; }
    /// <summary>
    /// All declared Methods in this class
    /// </summary>
    public List<ExtractedMethodInfo> Methods { get; set; } = new List<ExtractedMethodInfo>();
    /// <summary>
    /// All declared Properties in this Class.
    /// </summary>
    public List<ExtractedPropertyInfo> Properties { get; set; } = new List<ExtractedPropertyInfo>();
    /// <summary>
    /// All Classes that are defined within this Class.
    /// </summary>
    public List<ExtractedClassInfo> SubClasses { get; set; } = new List<ExtractedClassInfo>();
}