using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BoTech.AvaloniaDesigner.ViewModels.Editor;

namespace BoTech.AvaloniaDesigner.Views.Editor;

public partial class ItemsExplorerView : UserControl
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
}