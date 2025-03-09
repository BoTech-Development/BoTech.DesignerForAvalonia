using System.IO;
using System.Reactive;
using System.Runtime.Serialization;
using BoTech.DesignerForAvalonia.Controller.Editor;
using BoTech.DesignerForAvalonia.Services.PropertiesView;
using BoTech.DesignerForAvalonia.Views;
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
    /// The instance of the current Editor Controller, which is needed to open Views again.
    /// </summary>
    public EditorController EditorController { get; set; }
    public ReactiveCommand<Unit, Unit> LoadProjectCommand { get; set; }
    public ReactiveCommand<Unit, Unit> AboutViewCommand { get; set; }
    /// <summary>
    /// It is needed to have the MainViewModel reference to set the Content Object => Can change when the user wants to work either in the Editor or in the Style Editor.
    /// </summary>
    private MainViewModel MainViewModel { get; set; }

   
    public TopNavigationViewModel(MainViewModel mainViewModel)
    {
        LoadProjectCommand = ReactiveCommand.Create(LoadProject);
        AboutViewCommand = ReactiveCommand.Create(ShowAboutView);
        OpenViewCommand = ReactiveCommand.Create<string>(OpenView);
        MainViewModel = mainViewModel;
        
    }

    private void OpenView(string viewName)
    {
        switch (viewName)
        {
            case "Solution-Explorer":
                EditorController.SolutionExplorerView.Open();
                break;
            case "Items-Explorer":
                EditorController.ItemsView.Open();
                break;
            case "Preview-View":
                EditorController.PreviewView.Open();
                break;
            case "Hierarchy-View":
                EditorController.ViewHierarchyView.Open();
                break;
            case "Properties-View":
                EditorController.PropertiesView.Open();
                break;
        }
    }
    private void LoadProject()
    {
        new ProjectStartViewModel(MainViewModel).OpenProject();
    }

    private void ShowAboutView()
    {
        DialogHost.Show(new AboutView()
        {
            DataContext = new AboutViewModel(),
        }, "MainDialogHost");
    }
}