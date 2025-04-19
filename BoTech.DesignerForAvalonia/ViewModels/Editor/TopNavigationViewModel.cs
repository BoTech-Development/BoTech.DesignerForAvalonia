using System.IO;
using System.Reactive;
using System.Runtime.Serialization;
using Avalonia.Controls;
using Avalonia.Media;
using BoTech.DesignerForAvalonia.Controller.Editor;
using BoTech.DesignerForAvalonia.Models.Project;
using BoTech.DesignerForAvalonia.Services.PropertiesView;
using BoTech.DesignerForAvalonia.ViewModels.Editor.Dialogs;
using BoTech.DesignerForAvalonia.Views;
using BoTech.DesignerForAvalonia.Views.Editor;
using BoTech.DesignerForAvalonia.Views.Editor.Dialogs;
using DialogHostAvalonia;
using Material.Icons;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.ViewModels.Editor;

public class TopNavigationViewModel : ViewModelBase
{
    /// <summary>
    /// Shows the Dialog which can edit the Label Color
    /// </summary>
    public ReactiveCommand<Unit, Unit> ChangeProjectLabelColorCommand { get; set; }
    
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
    
    private bool _isProjectLoaded  = false;
    /// <summary>
    /// Is true when a project is loaded so that the View can show the name and can display some more Controls in the TopNavBar
    /// </summary>
    public bool IsProjectLoaded
    {
        get => _isProjectLoaded;
        set => this.RaiseAndSetIfChanged(ref _isProjectLoaded, value);
    }

    private Project _loadedProject = new();

    /// <summary>
    /// The Project which was Loaded. Must be initialized for the View, which access this Variable.
    /// </summary>
    public Project LoadedProject
    {
        get => _loadedProject;
        set => this.RaiseAndSetIfChanged(ref _loadedProject, value);
    }

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
        ChangeProjectLabelColorCommand = ReactiveCommand.Create(() =>
        {
            IsProjectLoaded = false;
            ChangeColorOfProject(LoadedProject);
            IsProjectLoaded = true;
        });
        _mainViewModel = mainViewModel;
        
    }

    public static void ChangeColorOfProject(Project project)
    {
        
        DialogHost.Show(new GenericDialogView()
        {
            DataContext = new GenericDialogViewModel()
            {
                Icon = MaterialIconKind.PaletteOutline,
                Content = new ProjectColorDialogView()
                {
                    DataContext = new ProjectColorDialogViewModel(project)
                }
            }
        });
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