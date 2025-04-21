using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using BoTech.DesignerForAvalonia.Models.Project.CSharp;


namespace BoTech.DesignerForAvalonia.Models.Project;
/// <summary>
/// Stores all important information for an ViewModel.
/// </summary>
public class ProjectViewModel
{
    /// <summary>
    /// The absolute Path to the ViewModel
    /// </summary>
    public required string Path { get; set; }
    /// <summary>
    /// The Name of the class
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// The full namespace of the class with the same Name as the File. The classname is included.
    /// </summary>
    public string Namespace { get; set; } = string.Empty;
    /// <summary>
    /// The parsed information from the .cs source File. 
    /// </summary>
    [JsonIgnore]
    public ExtractedClassInfo? ClassInfoFromFile { get; set; }
    /// <summary>
    /// Is true when all members, functions that are located in the Source File are also defined in the Assembly.
    /// </summary>
    [JsonIgnore]
    public bool IsAssemblyEqualsToSource { get; set; }
    
}