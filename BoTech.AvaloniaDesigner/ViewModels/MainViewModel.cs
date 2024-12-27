using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using BoTech.AvaloniaDesigner.Controller.Editor;
using BoTech.AvaloniaDesigner.Services.PropertiesView;
using BoTech.AvaloniaDesigner.ViewModels.Editor;
using BoTech.AvaloniaDesigner.Views.Editor;
using ReactiveUI;

namespace BoTech.AvaloniaDesigner.ViewModels;

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
        
        
        
        StatusConsoleView = new StatusConsoleView()
        {

        };
    }
}
