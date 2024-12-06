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

    public MainViewModel()
    {
        DragAndDropController dragAndDropController = new DragAndDropController();
        PreviewView = new PreviewView()
        {
            DataContext = new PreviewViewModel(dragAndDropController)
        };
        ItemsView = new ItemsExplorerView()
        {
            DataContext = new ItemsExplorerViewModel(dragAndDropController)
        };
    }
}
