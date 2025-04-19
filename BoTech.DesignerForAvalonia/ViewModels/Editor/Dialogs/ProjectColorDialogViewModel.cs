using System.Reactive;
using BoTech.DesignerForAvalonia.Models.Project;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.ViewModels.Editor.Dialogs;
/// <summary>
/// This View can be used to let the user decide which label-color he wants to use for th Project
/// </summary>
public class ProjectColorDialogViewModel : ViewModelBase
{
    public string ShortName { get; set; } = "Test";
    public ReactiveCommand<string, Unit> ChangeColorCommand { get; set; }
    public Project Project { get; set; }
    public ProjectColorDialogViewModel(Project project)
    {
        Project = project;
        ShortName = project.ShortName;
        ChangeColorCommand = ReactiveCommand.Create<string>(colorName => Project.DisplayableProjectInfo.SetColorByName(colorName));
    }
}