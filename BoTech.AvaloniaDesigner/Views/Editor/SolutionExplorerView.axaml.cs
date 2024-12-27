using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BoTech.AvaloniaDesigner.ViewModels.Editor;

namespace BoTech.AvaloniaDesigner.Views.Editor;

public partial class SolutionExplorerView : UserControl
{
    public SolutionExplorerView()
    {
        InitializeComponent();
    }

    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is SolutionExplorerViewModel vm)
        {
            vm.OnTreeViewNodeSelectedChanged();
        }
    }
}