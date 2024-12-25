using Avalonia;
using Avalonia.Controls;
using BoTech.AvaloniaDesigner.Controller.Editor;
using BoTech.AvaloniaDesigner.Services.PropertiesView;
using BoTech.AvaloniaDesigner.ViewModels.Editor;
using BoTech.AvaloniaDesigner.Views.Editor;

namespace BoTech.AvaloniaDesigner.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";
    public PreviewView PreviewView { get; set; }
    public ItemsExplorerView ItemsView { get; set; } 
    public ViewHierarchyView ViewHierarchyView { get; set; }
    public PropertiesView PropertiesView { get; set; }
    
 

    public MainViewModel()
    {
        PreviewController previewController = new PreviewController();
        PreviewView = new PreviewView()
        {
            DataContext = new PreviewViewModel(previewController)
        };
        ItemsView = new ItemsExplorerView()
        {
            DataContext = new ItemsExplorerViewModel(previewController)
        };
        ViewHierarchyView = new ViewHierarchyView()
        {
            DataContext = new ViewHierarchyViewModel(previewController)
        };
        PropertiesView = new PropertiesView()
        {
            DataContext = new PropertiesViewModel(previewController)
        };
        // Add the PreviewController Instance to the ControlsCreator class:
        ControlsCreator.PreviewController = previewController;
        previewController.Init();
        
    }
}
