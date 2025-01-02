using Avalonia.Controls;
using BoTech.AvaloniaDesigner.ViewModels;

namespace BoTech.AvaloniaDesigner.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DialogWindowViewModel.Instance.SetOwner(this);
    }
}
