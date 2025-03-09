using Avalonia.Controls;

namespace BoTech.DesignerForAvalonia.Views.Abstraction;

public class CloseablePageCodeBehind : UserControl
{
    public void Close()
    {
        this.IsVisible = false;
    }
    public void Open()
    {
        this.IsVisible = true;
    }
}