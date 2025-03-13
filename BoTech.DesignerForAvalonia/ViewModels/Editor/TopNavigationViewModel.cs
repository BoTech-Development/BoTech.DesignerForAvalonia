using System.IO;
using System.Reactive;
using System.Runtime.Serialization;
using BoTech.DesignerForAvalonia.Controller.Editor;
using BoTech.DesignerForAvalonia.Models.Project;
using BoTech.DesignerForAvalonia.Services.PropertiesView;
using BoTech.DesignerForAvalonia.Views;
using BoTech.DesignerForAvalonia.Views.Editor;
using DialogHostAvalonia;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.ViewModels.Editor;

public class TopNavigationViewModel : ViewModelBase
{
    /// <summary>
    /// This command Opens the view with the given name.
    /// </summary>
    public ReactiveCommand<string, Unit> OpenViewCommand { get; set; }
    /// <summary>
    /// Loads a new Porject
    /// </summary>
    public ReactiveCommand<Unit, Unit> LoadProjectCommand { get; set; }
    /// <summary>
    /// Shows the About View Dialog.
    /// </summary>
    public ReactiveCommand<Unit, Unit> AboutViewCommand { get; set; }
    /// <summary>
    /// Is true when a project is loaded so that the View can show the name and can display some more Controls in the TopNavBar
    /// </summary>
    public bool IsProjectLoaded { get; set; } = false;
    /// <summary>
    /// The Project which was Loaded. Must be initialized for the View, which access this Variable.
    /// </summary>
    public Project LoadedProject { get; set; } = new();
    /// <summary>
    /// It is needed to have the MainViewModel reference to set the Content Object => Can change when the user wants to work either in the Editor or in the Style Editor.
    /// </summary>
    private MainViewModel _mainViewModel { get; set; }
    private ProjectController _projectController { get; set; }
   
    public TopNavigationViewModel(MainViewModel mainViewModel)
    {
        LoadProjectCommand = ReactiveCommand.Create(LoadProject);
        AboutViewCommand = ReactiveCommand.Create(ShowAboutView);
        OpenViewCommand = ReactiveCommand.Create<string>(OpenView);
        _mainViewModel = mainViewModel;
        
    }
    /// <summary>
    /// Initialise the View => Shows for example the Current Opened project in the top nav bar.
    /// </summary>
    public void OnProjectLoaded(ProjectController projectController)
    {
        _projectController = projectController;
        LoadedProject = projectController.LoadedProject;
        IsProjectLoaded = true;
    }
    private void OpenView(string viewName)
    {
        switch (viewName)
        {
            case "Solution-Explorer":
                _projectController.EditorController.SolutionExplorerView.Open();
                break;
            case "Items-Explorer":
                _projectController.EditorController.ItemsView.Open();
                break;
            case "Preview-View":
                _projectController.EditorController.PreviewView.Open();
                break;
            case "Hierarchy-View":
                _projectController.EditorController.ViewHierarchyView.Open();
                break;
            case "Properties-View":
                _projectController.EditorController.PropertiesView.Open();
                break;
        }
    }
    private void LoadProject()
    {
        new ProjectStartViewModel(_mainViewModel).OpenProject();
    }

    private void ShowAboutView()
    {
        DialogHost.Show(new AboutView()
        {
            DataContext = new AboutViewModel(),
        }, "MainDialogHost");
    }
}