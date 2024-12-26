using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using BoTech.AvaloniaDesigner.Controller.Editor;
using BoTech.AvaloniaDesigner.Models.Editor;
using BoTech.AvaloniaDesigner.Services.Avalonia;
using ReactiveUI;

namespace BoTech.AvaloniaDesigner.ViewModels.Editor;

public class PreviewViewModel : ViewModelBase, INotifyPropertyChanged
{
/*
    /// <summary>
    /// Event will be called if the Preview changed
    /// </summary>
    public event EventHandler<Grid>? PreviewContentChanged;
    protected virtual void OnPreviewContentChanged(Grid previewContent)
    {
       /* EventHandler<Grid>? handler = PreviewContentChanged;
        if (handler != null)
        {
            handler(this, previewContent);
        }*/
    /*
       PreviewContentChanged?.Invoke(this, previewContent);
    }
    private Grid _previewContent;
    public Grid PreviewContent
    {
        get => _previewContent;
        set
        {
            // Call the Event.
            OnPreviewContentChanged(value);
            _previewContent = value;
        }
    }
*/


    /// <summary>
    /// Will be injected.
    /// </summary>
    public PreviewController PreviewController { get; set; }

    public PreviewViewModel(PreviewController previewController)
    {
        TextBlock pleaseWait = new TextBlock()
        {
            Text = "Please wait while initialisation...",
        };
        PreviewController = previewController;
        PreviewController.PreviewViewModel = this;
        PreviewController.PreviewContent = new Grid();
        PreviewController.PreviewContent.Children.Add(pleaseWait);
        
        //Init();
    }

    // Will be called from another Class
    private void Init()
    {

    }

    // Events:
    public void OnPointerMoved(PointerEventArgs e)
    {
        if (PreviewController.Operation == EDragAndDropOperation.DropObjectToPreview &&
            PreviewController.CurrentControl != null)
        {
            if(PreviewController.CurrentControl != null) TryToRemoveExistingControl(PreviewController.CurrentControl, PreviewController.PreviewContent);

            string error = "";
            if (!PlaceControlByPointerPosition(out error))
            {
                // Error Handling
            }

        }
    }
    /// <summary>
    /// Drag and Drop is Paused when it is running, until the user move the Pointer over the Preview.
    /// </summary>
    /// <param name="e"></param>
    public void OnPointerExited(PointerEventArgs e)
    {
        if (PreviewController.Operation == EDragAndDropOperation.DropObjectToPreview)
        {
            PreviewController.DraggingPaused();
        }
    }
    public void OnPointerPressed(PointerEventArgs e)
    {
        if (PreviewController.Operation == EDragAndDropOperation.DropObjectToPreview)
        {
            PreviewController.EndDrag();
        }
    }
    /// <summary>
    /// This Method is used to delete the new Control and place it to another position. To Place the control to a new position the Method <see cref="PlaceControlByPointerPosition"/> is used.
    /// </summary>
    /// <param name="control">The new Control</param>
    private void TryToRemoveExistingControl(Control control, Control previewControl)
    {
        if (TypeCastingService.IsLayoutControl(previewControl))
        {
            Controls? children;
            if ((children = TypeCastingService.GetChildControlsOfLayoutControl(previewControl)) != null)
            {
                if (children.Contains(control))
                {
                    children.Remove(control);
                }
                else
                {
                    // Go deeper in the Visual Tree
                    foreach (Control child in children)
                    {
                        TryToRemoveExistingControl(control, child);
                    }
                }
            }
        }
      
        
    }

    /// <summary>
    /// Tries to add the selected Control (located in the Drag and Drop Controller <see cref="PreviewController"/>), to the current Preview Context.<br/>
    ///  Method adds the Control to the Control or in the Layout Control where the Pointer points to.
    /// </summary>
    /// <param name="error">Can be used to get the error string, when the system can not place the Control there</param>
    /// <returns>Returns true when the Control has been added to the Grid or to the correct position.</returns>
    private bool PlaceControlByPointerPosition(out string error, Control? control = null)
    {
        bool placed = false;
        error = string.Empty;
       
        if (control == null)
        {
            control = PreviewController.PreviewContent;
            //error = "Error: Control can be not null at this point.";
            //return false; // Error Control can be not null at this point.
        }

        if (control.IsPointerOver)
        {
            if (TypeCastingService.IsLayoutControl(control))
            {
                if (PreviewController.CurrentControl != null)
                {
                    switch (control.GetType().Name)
                    {
                        case "Border":
                            ((Border)control).Child = PreviewController.CurrentControl;
                            placed = true;
                            break;
                        case "Canvas":
                            ((Canvas)control).Children.Add(PreviewController.CurrentControl);
                            placed = true;
                            break;
                        case "DockPanel":
                            ((DockPanel)control).Children.Add(PreviewController.CurrentControl);
                            placed = true;
                            break;
                        case "Expander":
                            ((Expander)control).Content = PreviewController.CurrentControl;
                            placed = true;
                            break;
                        case "Grid":
                            ((Grid)control).Children.Add(PreviewController.CurrentControl);
                            placed = true;
                            break;
                        case "GridSplitter":
                            throw new NotSupportedException(
                                "The Control Grid Splitter is not supported in this Version");
                            //  ((GridSplitter)control)..Add(PreviewController.CurrentControl); 
                            break;
                        case "Panel":
                            ((Panel)control).Children.Add(PreviewController.CurrentControl);
                            placed = true;
                            break;
                        case "RelativePanel":
                            ((RelativePanel)control).Children.Add(PreviewController.CurrentControl);
                            placed = true;
                            break;
                        case "ScrollViewer":
                            ((ScrollViewer)control).Content = PreviewController.CurrentControl;
                            placed = true;
                            break;
                        case "SplitView":
                            throw new NotSupportedException("The Control SplitView is not supported in this Version");
                            // TODO: Add MessageBox to let the user decide if he wants to place the new Object in the Content or Pane side.
                            // ((SplitView)control)..Add(PreviewController.CurrentControl); 
                            break;
                        case "TabControl":
                            throw new NotSupportedException("The Control TabControl is not supported in this Version");
                            // TODO: Implement the TabControl
                            // ((TabControl)control)..Add(PreviewController.CurrentControl); 
                            break;
                        case "UniformGrid":
                            ((UniformGrid)control).Children.Add(PreviewController.CurrentControl);
                            placed = true;
                            break;
                        case "WrapPanel":
                            ((WrapPanel)control).Children.Add(PreviewController.CurrentControl);
                            placed = true;
                            break;

                    }
                }

            }
            else
            {
                // when it is a normal Control

            }
        }
        else
        {
            // When the User does not point to it
            Controls? children;
            if ((children = TypeCastingService.GetChildControlsOfLayoutControl(control)) != null)
            {
                // Go deeper in the Visual Tree
                foreach (Control child in children)
                {
                    return PlaceControlByPointerPosition(out error, child);
                }
            }
        }

        // When the System could not find any Children
        // place it into the Grid:
        if (!placed)
        {
            if (PreviewController.CurrentControl != null)
            {
                PreviewController.PreviewContent.Children.Add(PreviewController.CurrentControl);
                // TODO: Call this Method by an Event
                PreviewController.OnPreviewContentChanged();
            }
        }
        else
        {
            PreviewController.OnPreviewContentChanged();
        }
        
        return false;
    }


   


}