using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BoTech.DesignerForAvalonia.ViewModels.Editor;
using BoTech.DesignerForAvalonia.Views.Abstraction;

namespace BoTech.DesignerForAvalonia.Views.Editor;

public partial class SolutionExplorerView : CloseablePageCodeBehind
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

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is SolutionExplorerViewModel vm)
        {
            vm.OnExplorerTypeChanged();
        }
    }
}