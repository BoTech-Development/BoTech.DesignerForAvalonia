using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BoTech.DesignerForAvalonia.ViewModels.Editor;
using BoTech.DesignerForAvalonia.Views.Abstraction;

namespace BoTech.DesignerForAvalonia.Views.Editor;

public partial class ItemsExplorerView : CloseablePageCodeBehind
{
    public ItemsExplorerView()
    {
        InitializeComponent();
    }

    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is ItemsExplorerViewModel vm)
        {
            vm.OnTreeViewSelectionChanged();
        }
    }

    private void AutoCompleteBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (DataContext is ItemsExplorerViewModel vm)
        {
            vm.Search();
        }
    }
}