using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BoTech.DesignerForAvalonia.Models.Project.CSharp;

public class ExtractedMethodInfo
{
    /// <summary>
    /// The Name of the Method 
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// Return Type name with namespace if necessary
    /// </summary>
    public string ReturnType { get; set; } = "void";
    /// <summary>
    /// The xml Documentation
    /// </summary>
    public string Documentation { get; set; } = string.Empty;
    /// <summary>
    /// All Parameters of the Method
    /// </summary>
    public List<ExtractedParamInfo> Parameters { get; set; } = new List<ExtractedParamInfo>();
    /// <summary>
    /// If an exception is thrown during the parsing process, the error text can be stored here.
    /// </summary>
    [JsonIgnore]
    public string ParseError { get; set; } = string.Empty;
}

public class ExtractedParamInfo
{
    /// <summary>
    /// The name of the Parameter
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// Type name with namespace if necessary
    /// </summary>
    public required string Type { get; set; }
    /// <summary>
    /// Type name with namespace if necessary
    /// </summary>
    public required string DefaultValue { get; set; }
    /// <summary>
    /// extracted Documentation of the Method documentation
    /// </summary>
    public required string Documentation { get; set; }
    /// <summary>
    /// If an exception is thrown during the parsing process, the error text can be stored here.
    /// </summary>
    [JsonIgnore]
    public string ParseError { get; set; } = string.Empty;
}