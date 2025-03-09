using System.Reactive;
using BoTech.DesignerForAvalonia.ViewModels;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.Models.Project;
/// <summary>
/// This class implementes the LoadRecentProjectCommand
/// </summary>
public class OpenableProject : Project
{
    /// <summary>
    /// Command for the ItemsControl at the Project start Page
    /// </summary>
    public ReactiveCommand<Unit, Unit> LoadRecentProjectCommand { get; set; }

    public OpenableProject(ProjectStartViewModel viewModel, Project project)
    {
        // Copy all Properties of base class
        this.SolutionFilePath = project.SolutionFilePath;
        this.OutputPath = project.OutputPath;
        this.LastUsed = project.LastUsed;
        this.Name = project.Name;
        this.ProjectViews = project.ProjectViews;
        this.ViewModelPath = project.ViewModelPath;
        this.SolutionFile = project.SolutionFile;
        this.ViewPath = project.ViewPath;
        this.ShortName = project.ShortName;
        LoadRecentProjectCommand = ReactiveCommand.Create(() =>
        {
            viewModel.LoadProject(this as Project);
        });
    }
}