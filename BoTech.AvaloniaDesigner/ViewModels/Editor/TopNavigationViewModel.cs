using System.IO;
using System.Reactive;
using System.Runtime.Serialization;
using BoTech.AvaloniaDesigner.Controller.Editor;
using BoTech.AvaloniaDesigner.Services.PropertiesView;
using ReactiveUI;

namespace BoTech.AvaloniaDesigner.ViewModels.Editor;

public class TopNavigationViewModel : ViewModelBase
{
    public string SelectedPath { get; set; } = string.Empty;
    public ReactiveCommand<Unit, Unit> LoadNewDirectoryCommand { get; set; }
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
                MainViewModel.Content = EditorController.Init(SelectedPath);
                // Add the EditorController Instance to the ControlsCreator class:
                ControlsCreator.EditorController = EditorController;
            }
        }
    }
    
}