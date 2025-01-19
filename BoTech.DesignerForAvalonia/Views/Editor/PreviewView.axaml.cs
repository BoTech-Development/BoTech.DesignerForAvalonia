using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using BoTech.DesignerForAvalonia.ViewModels.Editor;

namespace BoTech.DesignerForAvalonia.Views.Editor;

public partial class PreviewView : UserControl
{
    public PreviewView()
    {
        InitializeComponent();
    }

    private void Preview_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (DataContext is PreviewViewModel vm)
        {
            vm.OnPointerMoved(e);
        }
    }

    private void Preview_OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (DataContext is PreviewViewModel vm)
        {
            vm.OnPointerExited(e);
        }
    }

    private void Preview_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is PreviewViewModel vm)
        {
            vm.OnPointerPressed(e);
        }
    }
}