using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using BoTech.DesignerForAvalonia.Controller.Editor;
using BoTech.DesignerForAvalonia.Services.PropertiesView;
using BoTech.DesignerForAvalonia.Services.XML;
using BoTech.DesignerForAvalonia.ViewModels.Editor;
using BoTech.DesignerForAvalonia.Views;
using BoTech.DesignerForAvalonia.Views.Editor;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    public TopNavigationView TopNavigationView { get; set; }

    private Control _content = new TextBlock()
    {
        Text = "Please Open a Directory",
        FontWeight = FontWeight.Bold,
        Foreground = Brushes.Orange,
    };

    /// <summary>
    /// Here all Views will be injected with a Grid.
    /// </summary>
    public Control Content
    {
        get => _content; 
        set => this.RaiseAndSetIfChanged(ref _content, value);
    } 
    public StatusConsoleView StatusConsoleView { get; set; }

    public MainViewModel()
    {
   
      
        TopNavigationView = new TopNavigationView()
        {
            DataContext = new TopNavigationViewModel(this)
        };

       
        _content = new ProjectStartView()
        {
            DataContext = new ProjectStartViewModel(this)
        };
        
        StatusConsoleView = new StatusConsoleView()
        {

        };
    }
}
