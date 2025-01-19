using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BoTech.DesignerForAvalonia.ViewModels.Editor;

namespace BoTech.DesignerForAvalonia.Views.Editor;

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