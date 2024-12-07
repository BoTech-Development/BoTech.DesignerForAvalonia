using Avalonia.Controls;
using BoTech.AvaloniaDesigner.Models.Editor;
using BoTech.AvaloniaDesigner.ViewModels.Editor;

namespace BoTech.AvaloniaDesigner.Controller.Editor;

public class PreviewController
{
    // All View Models which are Related to the Controller:
    // Each ViewModel has to set its Property in its ctor.
    public PreviewViewModel? PreviewViewModel { get; set; }
    public ItemsExplorerViewModel? ItemsExplorerViewModel { get; set; }
    public ViewHierarchyViewModel? ViewHierarchyViewModel { get; set; }
    public PropertiesViewModel? PropertiesViewModel { get; set; }
    
    /// <summary>
    /// What the user want to do
    /// </summary>
    public EDragAndDropOperation Operation { get; set; }
    /// <summary>
    /// Selected Control which can be dropped to the Preview.
    /// </summary>
    public Control? CurrentControl { get; set; }
    /// <summary>
    /// Will be set by the <see cref="PreviewViewModel"/> when the user hase changed something.
    /// </summary>
    public Grid PreviewContent { get; set; }
    
    private Grid OriginalContent { get; set; }
    
    public PreviewController()
    {
        Operation = EDragAndDropOperation.None;
        CurrentControl = null;
    }
    /// <summary>
    /// The Controller has to be Initialized before it can run.
    /// </summary>
    public void Init()
    {
        if (PreviewViewModel != null)
        {
            PreviewViewModel.PreviewContentChanged += OnPreviewContentChanged;
        }
    }
    /// <summary>
    /// Will be executed when the PreviewContent was changed by the PreviewViewModel.
    /// This Event is needed, because other Views like the ViewHierarchy View has to reload the Tree View.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnPreviewContentChanged(object? sender, Grid e)
    {
        if(ViewHierarchyViewModel != null) ViewHierarchyViewModel.Reload();
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