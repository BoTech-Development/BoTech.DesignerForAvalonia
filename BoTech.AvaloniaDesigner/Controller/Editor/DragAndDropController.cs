using Avalonia.Controls;
using BoTech.AvaloniaDesigner.Models.Editor;

namespace BoTech.AvaloniaDesigner.Controller.Editor;

public class DragAndDropController
{
    public EDragAndDropOperation Operation { get; set; }
    public Control? CurrentControl { get; set; }

    public DragAndDropController()
    {
        Operation = EDragAndDropOperation.None;
        CurrentControl = null;
    }

    public void StartDrag(Control? control)
    {
        CurrentControl = control;
        Operation = EDragAndDropOperation.DropObjectToPreview;
    }

    public void DraggingPaused()
    {
        
    }
    public void EndDrag()
    {
        CurrentControl = null;
    }
}