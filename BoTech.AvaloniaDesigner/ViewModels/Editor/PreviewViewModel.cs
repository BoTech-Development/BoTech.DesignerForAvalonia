using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using BoTech.AvaloniaDesigner.Controller.Editor;
using BoTech.AvaloniaDesigner.Models.Editor;
using ReactiveUI;

namespace BoTech.AvaloniaDesigner.ViewModels.Editor;

public class PreviewViewModel : ViewModelBase
{
    private Grid _previewContent;

    public Grid PreviewContent
    {
        get => _previewContent;
        set => this.RaiseAndSetIfChanged(ref _previewContent, value);
    }

    private Grid OriginalContent { get; set; }

    /// <summary>
    /// Will be injected.
    /// </summary>
    private DragAndDropController _dragAndDropController;

    public PreviewViewModel(DragAndDropController dragAndDropController)
    {
        TextBlock pleaseWait = new TextBlock()
        {
            Text = "Please wait while initialisation...",
        };
        _previewContent = new Grid();
        _previewContent.Children.Add(pleaseWait);
        _dragAndDropController = dragAndDropController;
        //Init();
    }

    // Will be called from another Class
    private void Init()
    {

    }

    // Events:
    public void OnPointerMoved(PointerEventArgs e)
    {
        if (_dragAndDropController.Operation == EDragAndDropOperation.DropObjectToPreview &&
            _dragAndDropController.CurrentControl != null)
        {
            if(_dragAndDropController.CurrentControl != null) TryToRemoveExistingControl(_dragAndDropController.CurrentControl, PreviewContent);

            string error = "";
            if (!PlaceControlByPointerPosition(out error))
            {
                // Error Handeling
            }

        }
    }
    /// <summary>
    /// Drag and Drop is Paused when it is running, until the user move the Pointer over the Preview.
    /// </summary>
    /// <param name="e"></param>
    public void OnPointerExited(PointerEventArgs e)
    {
        if (_dragAndDropController.Operation == EDragAndDropOperation.DropObjectToPreview)
        {
            _dragAndDropController.DraggingPaused();
        }
    }
    public void OnPointerPressed(PointerEventArgs e)
    {
        if (_dragAndDropController.Operation == EDragAndDropOperation.DropObjectToPreview)
        {
            _dragAndDropController.EndDrag();
        }
    }
    /// <summary>
    /// This Method is used to delete the new Control and place it to another position. To Place the control to a new position the Method <see cref="PlaceControlByPointerPosition"/> is used.
    /// </summary>
    /// <param name="control">The new Control</param>
    private void TryToRemoveExistingControl(Control control, Control previewControl)
    {
        if (IsLayoutControl(previewControl))
        {
            Controls? children;
            if ((children = GetChildControlsOfLayoutControl(previewControl)) != null)
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
    /// Tries to add the selected Control (located in the Drag and Drop Controller <see cref="DragAndDropController"/>), to the current Preview Context.<br/>
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
            control = PreviewContent;
            //error = "Error: Control can be not null at this point.";
            //return false; // Error Control can be not null at this point.
        }

        if (control.IsPointerOver)
        {
            if (IsLayoutControl(control))
            {
                if (_dragAndDropController.CurrentControl != null)
                {
                    switch (control.GetType().Name)
                    {
                        case "Border":
                            ((Border)control).Child = _dragAndDropController.CurrentControl;
                            placed = true;
                            break;
                        case "Canvas":
                            ((Canvas)control).Children.Add(_dragAndDropController.CurrentControl);
                            placed = true;
                            break;
                        case "DockPanel":
                            ((DockPanel)control).Children.Add(_dragAndDropController.CurrentControl);
                            placed = true;
                            break;
                        case "Expander":
                            ((Expander)control).Content = _dragAndDropController.CurrentControl;
                            placed = true;
                            break;
                        case "Grid":
                            ((Grid)control).Children.Add(_dragAndDropController.CurrentControl);
                            placed = true;
                            break;
                        case "GridSplitter":
                            throw new NotSupportedException(
                                "The Control Grid Splitter is not supported in this Version");
                            //  ((GridSplitter)control)..Add(_dragAndDropController.CurrentControl); 
                            break;
                        case "Panel":
                            ((Panel)control).Children.Add(_dragAndDropController.CurrentControl);
                            placed = true;
                            break;
                        case "RelativePanel":
                            ((RelativePanel)control).Children.Add(_dragAndDropController.CurrentControl);
                            placed = true;
                            break;
                        case "ScrollViewer":
                            ((ScrollViewer)control).Content = _dragAndDropController.CurrentControl;
                            placed = true;
                            break;
                        case "SplitView":
                            throw new NotSupportedException("The Control SplitView is not supported in this Version");
                            // TODO: Add MessageBox to let the user decide if he wants to place the new Object in the Content or Pane side.
                            // ((SplitView)control)..Add(_dragAndDropController.CurrentControl); 
                            break;
                        case "TabControl":
                            throw new NotSupportedException("The Control TabControl is not supported in this Version");
                            // TODO: Implement the TabControl
                            // ((TabControl)control)..Add(_dragAndDropController.CurrentControl); 
                            break;
                        case "UniformGrid":
                            ((UniformGrid)control).Children.Add(_dragAndDropController.CurrentControl);
                            placed = true;
                            break;
                        case "WrapPanel":
                            ((WrapPanel)control).Children.Add(_dragAndDropController.CurrentControl);
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
            if ((children = GetChildControlsOfLayoutControl(control)) != null)
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
        if(!placed) if(_dragAndDropController.CurrentControl != null) PreviewContent.Children.Add(_dragAndDropController.CurrentControl);
        return false;
    }


    /// <summary>
    /// Checks if the given Control is a Layout Control ("Border", "Canvas", "DockPanel", "Expander", "Grid", "GridSplitter", "Panel", "RelativePanel", "ScrollViewer", "SplitView", "StackPanel", "TabControl", "UniformGrid", "WrapPanel") and has the <b>Childs</b> Property.
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    private bool IsLayoutControl(Control control)
    {
        // Possible Layout Types
        string[] layoutControlNames = ["Border", "Canvas", "DockPanel", "Expander", "Grid", "GridSplitter", "Panel", "RelativePanel", "ScrollViewer", "SplitView", "StackPanel", "TabControl", "UniformGrid", "WrapPanel"];
        if (layoutControlNames.Contains(control.GetType().Name)) return true; //control.GetType().GetField("Childs") != null;
        return false;
    }

    private Controls? GetChildControlsOfLayoutControl(Control control)
    {
        if (IsLayoutControl(control))
        {
            // Try Different Names for the Same Attribute:
            PropertyInfo[] propertyInfos = control.GetType().GetProperties();
            PropertyInfo? info = propertyInfos.Where(p => p.Name == "Children").FirstOrDefault();
            
            if (info == null) info = propertyInfos.Where(p => p.Name == "Child").FirstOrDefault();
            if (info == null) info = propertyInfos.Where(p => p.Name == "Content").FirstOrDefault();
            
            if (info != null)
            {
                object? obj = info.GetValue(control);
                if (obj != null)
                {
                    return (Controls)obj;
                }
            }
        }
        return null;
    }
}