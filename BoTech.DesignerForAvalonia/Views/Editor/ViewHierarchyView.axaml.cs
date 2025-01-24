using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using BoTech.DesignerForAvalonia.ViewModels.Editor;

namespace BoTech.DesignerForAvalonia.Views.Editor;

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