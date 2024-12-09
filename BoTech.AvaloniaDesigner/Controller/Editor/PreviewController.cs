using System;
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
    /// The Selected Control which the User can Select from the Hierarchy View
    /// </summary>
    public Control? SelectedControl { get; set; }

    private Grid _previewContent;

    /// <summary>
    /// Will be set by the <see cref="PreviewViewModel"/> when the user hase changed something.
    /// </summary>
    public Grid PreviewContent { get; set; }
   
    
    /*/// <summary>
    /// Event will be called if the Preview had changed
    /// </summary>
    public event EventHandler<Grid>? PreviewContentChanged;
    protected virtual void OnPreviewContentChanged(Grid previewContent)
    {
        PreviewContentChanged?.Invoke(this, previewContent);
    }*/
    
    
    
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
           // PreviewViewModel.PreviewContentChanged += OnPreviewContentChanged;
        }
    }

    /// <summary>
    /// Will be executed when the PreviewContent was changed by the PreviewViewModel.
    /// <b>Important this Method must be called by each class which change the Preview Content.</b>
    /// This Event is needed, because other Views like the ViewHierarchy View has to reload the Tree View.
    /// </summary>
    public void OnPreviewContentChanged()
    {
        if (ViewHierarchyViewModel != null) ViewHierarchyViewModel.Reload();
    }
    /// <summary>
    /// Event which will be called by the ViewHierarchyViewModel 
    /// </summary>
    public void OnSelectedControlChanged()
    {
        if (PropertiesViewModel != null && SelectedControl != null) PropertiesViewModel.RenderForControl(SelectedControl);
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