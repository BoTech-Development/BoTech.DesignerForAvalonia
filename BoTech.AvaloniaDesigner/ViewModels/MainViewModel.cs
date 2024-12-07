using Avalonia.Controls;
using BoTech.AvaloniaDesigner.Controller.Editor;
using BoTech.AvaloniaDesigner.ViewModels.Editor;
using BoTech.AvaloniaDesigner.Views.Editor;

namespace BoTech.AvaloniaDesigner.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";
    public PreviewView PreviewView { get; set; }
    public ItemsExplorerView ItemsView { get; set; } 
    public ViewHierarchyView ViewHierarchyView { get; set; }

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
    }
}
