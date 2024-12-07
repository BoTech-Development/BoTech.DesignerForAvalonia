using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BoTech.AvaloniaDesigner.ViewModels.Editor;

namespace BoTech.AvaloniaDesigner.Views.Editor;

public partial class ViewHierarchyView : UserControl
{
    public ViewHierarchyView()
    {
        InitializeComponent();
    }

    private void TreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is ViewHierarchyViewModel vm)
        {
            vm.OnTreeViewSelectionChanged();
        }
    }
}