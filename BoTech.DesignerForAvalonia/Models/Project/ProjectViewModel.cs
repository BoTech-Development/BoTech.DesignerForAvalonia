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
    [JsonIgnore]
    public ExtractedClassInfo? ClassInfoFromFile { get; set; }
    [JsonIgnore]
    public ExtractedClassInfo? ClassInfoFromDLL { get; set; }
    
    /// <summary>
    /// All Properties which are defined in the ViewModel .cs File. => When the project is loaded, the system searches the .cs file for properties and adds them to this list. 
    /// </summary>
  //  public List<ExtractedPropertyInfo> Properties { get; set; } = new List<ExtractedPropertyInfo>();
    /// <summary>
    /// Subset of the Properties Collection. Here all Properties which are present in the compiled class (.dll-File) are stored.
    /// </summary>
    //public List<ExtractedPropertyInfo> PropertiesDefinedInDLL { get; set; } = new List<ExtractedPropertyInfo>();

}