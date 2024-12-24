using Avalonia;
using Avalonia.Controls;
using BoTech.AvaloniaDesigner.ViewModels;

namespace BoTech.AvaloniaDesigner.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        this.PropertyChanged += MainWindow_PropertyChanged;
    }
    private void MainWindow_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (DataContext != null)
        {
            if (DataContext is MainViewModel vm)
            {
                if(e.Property.Name == "Bounds")
                    vm.Bounds = this.Bounds;
            }
        }
    }
}
