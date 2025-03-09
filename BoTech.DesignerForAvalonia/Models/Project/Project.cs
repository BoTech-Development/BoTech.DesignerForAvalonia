using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Build.Construction;

namespace BoTech.DesignerForAvalonia.Models.Project;

public class Project
{
    /// <summary>
    /// The Path to the .sln File.
    /// </summary>
    public string SolutionFilePath { get; set; } = string.Empty;
    /// <summary>
    /// The Model which contains Information about the Solution File. For example, it contains all defined Projects in the Solution.
    /// </summary>
    [JsonIgnore]
    public SolutionFile SolutionFile { get; set; }
    /// <summary>
    /// The Name of the Solution
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// A Short name which will be displayed in a Rounded Colored rectangle.
    /// </summary>
    public string ShortName { get; set; } = string.Empty;
    /// <summary>
    /// The Date and Time when the project was opened.
    /// </summary>
    public DateTime LastUsed { get; set; }
    /// <summary>
    /// The complete Path to the ViewModel folder.
    /// </summary>
    public string ViewModelPath { get; set; } = string.Empty;
    /// <summary>
    /// The complete Path to the Views folder.
    /// </summary>
    public string ViewPath { get; set; } = string.Empty;
    /// <summary>
    /// Saves the Path to the prebuilt assemblies, which contains all Classes and Views of the Project, so that they can be instantiated.
    /// </summary>
    public string OutputPath { get; set; } = string.Empty;
    /// <summary>
    /// All Views that the Project contains. By default, this will be the MainView and the MainWindow.
    /// </summary>
    public List<ProjectView> ProjectViews { get; set; } = new List<ProjectView>();
    public DisplayableProjectInfo DisplayableProjectInfo { get; set; } = new DisplayableProjectInfo();
    
}