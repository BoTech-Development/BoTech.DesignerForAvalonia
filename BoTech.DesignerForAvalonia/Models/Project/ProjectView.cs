using System;

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
    /// Complete Path to the {...}ViewModel.cs File
    /// </summary>
    public string PathToViewModel { get; set; }
}