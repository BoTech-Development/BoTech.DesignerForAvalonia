using System;
using System.Collections.Generic;
using BindingTest.Models;

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

}