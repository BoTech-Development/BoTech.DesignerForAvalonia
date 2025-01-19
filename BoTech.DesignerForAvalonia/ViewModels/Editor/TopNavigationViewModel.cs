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
    public string SelectedPath { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public ReactiveCommand<Unit, Unit> LoadNewDirectoryCommand { get; set; }
    public ReactiveCommand<Unit, Unit> AboutViewCommand { get; set; }
    /// <summary>
    /// It is needed to have the MainViewModel reference to set the Content Object => Can change when the user wants to work either in the Editor or in the Style Editor.
    /// </summary>
    public MainViewModel MainViewModel { get; set; }
    /// <summary>
    /// The Controller which manages the Editor Views ( PreviewView, PropertiesView, ViewHierarchyView, ItemsView and SolutionExplorerView) 
    /// </summary>
    private EditorController? EditorController { get; set; } = null;
    public TopNavigationViewModel(MainViewModel mainViewModel)
    {
        LoadNewDirectoryCommand = ReactiveCommand.Create(LoadNewDirectory);
        AboutViewCommand = ReactiveCommand.Create(ShowAboutView);
        MainViewModel = mainViewModel;
    }

    private void LoadNewDirectory()
    {
        if (SelectedPath != "")
        {
            if (Directory.Exists(SelectedPath))
            {
                // Creating an EditorController and update the Content Value.
                EditorController = new EditorController();
                MainViewModel.Content = EditorController.Init(SelectedPath, ProjectName);
                // Add the EditorController Instance to the ControlsCreator class:
                ControlsCreator.EditorController = EditorController;
            }
        }
    }

    private void ShowAboutView()
    {
        DialogHost.Show(new AboutView()
        {
            DataContext = new AboutViewModel(),
        }, "MainDialogHost");
    }
}