using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using BoTech.DesignerForAvalonia.ViewModels.Editor;
using BoTech.DesignerForAvalonia.Views.Abstraction;

namespace BoTech.DesignerForAvalonia.Views.Editor;

public partial class ViewHierarchyView : CloseablePageCodeBehind
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