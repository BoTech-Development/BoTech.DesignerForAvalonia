using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using BoTech.DesignerForAvalonia.Models.Project.CSharp;

namespace BoTech.DesignerForAvalonia.Models.Project;

public class ProjectView
{
    /// <summary>
    /// Name of the View 
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Complete Path to the .axaml File
    /// </summary>
    public string PathToView { get; set; }
    /// <summary>
    /// Complete Path to the .axaml.cs File
    /// </summary>
    public string PathToCodeBehind { get; set; }
    /// <summary>
    /// The ViewModel which is selected in the .axaml file.
    /// </summary>
    public ProjectViewModel ViewModel { get; set; } = null;
    /// <summary>
    /// All Bindings defined in this View.
    /// </summary>
    public List<Binding> Bindings { get; set; } = new List<Binding>();
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