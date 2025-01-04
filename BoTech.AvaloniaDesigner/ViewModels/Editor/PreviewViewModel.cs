using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using BoTech.AvaloniaDesigner.Controller.Editor;
using BoTech.AvaloniaDesigner.Models.Editor;
using BoTech.AvaloniaDesigner.Models.XML;
using BoTech.AvaloniaDesigner.Services.Avalonia;
using BoTech.AvaloniaDesigner.Services.XML;
using BoTech.AvaloniaDesigner.Views;
using DialogHostAvalonia;
using ReactiveUI;

namespace BoTech.AvaloniaDesigner.ViewModels.Editor;

public class PreviewViewModel : ViewModelBase, INotifyPropertyChanged
{
    /// <summary>
    /// when 
    /// </summary>
    private XmlControl? _oldConnectedXmlControl = null;
    /// <summary>
    /// Will be injected.
    /// </summary>
    public EditorController EditorController { get; set; }
    /// <summary>
    /// Will be called when the user clicks the Save Button.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SaveCommand { get; set; }
    /// <summary>
    /// Will be called when the User Clicks the Reload Button.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ReLoadCommand { get; set; }
    /// <summary>
    /// The Layout Control where the new Control was injected.
    /// Must be store because it is used by the CreateXmlControlConnection Method
    /// </summary>
    private Control _currentLayoutControl { get; set; }
    public PreviewViewModel(EditorController editorController)
    {
        EditorController = editorController;
        SaveCommand = ReactiveCommand.Create(OnSaveCommand);
        ReLoadCommand = ReactiveCommand.Create(OnReLoadCommand);
    }
    // Events:
    public void OnSaveCommand()
    {
        if (File.Exists(EditorController.OpenedFilePath))
        {
            LoadingViewModel loadingViewModel = new LoadingViewModel()
            {
                StatusText = "Saving... ",
                SubStatusText = "Updating XML...",
            };
            LoadingView view = new LoadingView()
            {
                DataContext = loadingViewModel,
            };
            DialogHost.Show(view, "MainDialogHost");
            loadingViewModel.SubStatusText = "Saving new Xml to File: " + EditorController.OpenedFilePath;
            
            string xml = new Serializer(loadingViewModel).Serialize(EditorController.RootConnectedNode);
            File.WriteAllText(EditorController.OpenedFilePath, xml);
            
            // Closing the new MessageBox (Loading View)
            DialogHost.Close("MainDialogHost");
        }
    }

    public void OnReLoadCommand()
    {
        
    }
    
    public void OnPointerMoved(PointerEventArgs e)
    {
        if (EditorController.Operation == EDragAndDropOperation.DropObjectToPreview &&
            EditorController.CurrentControl != null)
        {
            if(EditorController.CurrentControl != null) TryToRemoveExistingControl(EditorController.CurrentControl, EditorController.PreviewContent);

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
        if (EditorController.Operation == EDragAndDropOperation.DropObjectToPreview)
        {
            // Remove the new Control to visualize that it is not placed.
            if (EditorController.CurrentControl != null)
                TryToRemoveExistingControl(EditorController.CurrentControl, EditorController.PreviewContent);
            // EditorController.DraggingPaused();
        }
    }
    public void OnPointerPressed(PointerEventArgs e)
    {
        if (EditorController.Operation == EDragAndDropOperation.DropObjectToPreview)
        {
            CreateXmlControlConnection(_currentLayoutControl, EditorController.CurrentControl);
            EditorController.EndDrag();
        }
    }

    /// <summary>
    /// This Method is used to delete the new Control and place it to another position. To Place the control to a new position the Method <see cref="PlaceControlByPointerPosition"/> is used.
    /// </summary>
    /// <param name="control">The new Control</param>
    /// <param name="previewControl">The Preview Content or the next recursive Control.</param>
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
    /// Tries to add the selected Control (located in the Drag and Drop Controller <see cref="EditorController"/>), to the current Preview Context.<br/>
    ///  Method adds the SelectedControl to the Layout Control where the pointer is over.
    /// </summary>
    /// <param name="error">Can be used to get the error string, when the system can not place the Control there</param>
    /// <param name="control">For the recursive Call</param>
    /// <returns>Returns true when the Control has been added to the Grid or to the correct position.</returns>
    private bool PlaceControlByPointerPosition(out string error, Control? control = null)
    {
        bool placed = false;
        error = string.Empty;
       
        if (control == null)
        {
            control = EditorController.PreviewContent;
            //error = "Error: Control can be not null at this point.";
            //return false; // Error Control can be not null at this point.
        }

        if (control.IsPointerOver)
        {
            if (TypeCastingService.IsLayoutControl(control))
            {
                if (EditorController.CurrentControl != null)
                {
                    switch (control.GetType().Name)
                    {
                        case "Border":
                            ((Border)control).Child = EditorController.CurrentControl;
                            placed = true;
                            break;
                        case "Canvas":
                            ((Canvas)control).Children.Add(EditorController.CurrentControl);
                            placed = true;
                            break;
                        case "DockPanel":
                            ((DockPanel)control).Children.Add(EditorController.CurrentControl);
                            placed = true;
                            break;
                        case "Expander":
                            ((Expander)control).Content = EditorController.CurrentControl;
                            placed = true;
                            break;
                        case "Grid":
                            ((Grid)control).Children.Add(EditorController.CurrentControl);
                            placed = true;
                            break;
                        case "GridSplitter":
                            throw new NotSupportedException(
                                "The Control Grid Splitter is not supported in this Version");
                            //  ((GridSplitter)control)..Add(EditorController.CurrentControl); 
                            break;
                        case "Panel":
                            ((Panel)control).Children.Add(EditorController.CurrentControl);
                            placed = true;
                            break;
                        case "RelativePanel":
                            ((RelativePanel)control).Children.Add(EditorController.CurrentControl);
                            placed = true;
                            break;
                        case "ScrollViewer":
                            ((ScrollViewer)control).Content = EditorController.CurrentControl;
                            placed = true;
                            break;
                        case "SplitView":
                            throw new NotSupportedException("The Control SplitView is not supported in this Version");
                            // TODO: Add MessageBox to let the user decide if he wants to place the new Object in the Content or Pane side.
                            // ((SplitView)control)..Add(EditorController.CurrentControl); 
                            break;
                        case "TabControl":
                            throw new NotSupportedException("The Control TabControl is not supported in this Version");
                            // TODO: Implement the TabControl
                            // ((TabControl)control)..Add(EditorController.CurrentControl); 
                            break;
                        case "UniformGrid":
                            ((UniformGrid)control).Children.Add(EditorController.CurrentControl);
                            placed = true;
                            break;
                        case "WrapPanel":
                            ((WrapPanel)control).Children.Add(EditorController.CurrentControl);
                            placed = true;
                            break;
                        case "StackPanel":
                            ((StackPanel)control).Children.Add(EditorController.CurrentControl);
                            placed = true;
                            break;

                    }
                    // Save it for the CreateXmlControlConnection Method
                    _currentLayoutControl = control;
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
            if (EditorController.CurrentControl != null)
            {
             //   EditorController.PreviewContent.Children.Add(EditorController.CurrentControl);
                // TODO: Call this Method by an Event
                EditorController.OnPreviewContentChanged();
            }
        }
        else
        {
            EditorController.OnPreviewContentChanged();
        }
        
        return false;
    }
    /// <summary>
    /// Creates a new Connection between a new XmlNode and the new Control that the user has added.
    /// </summary>
    /// <param name="layoutControl">The new Control will be added to this Layout Control</param>
    /// <param name="newControl">The new Control that the User has selected.</param>
    private void CreateXmlControlConnection(Control layoutControl, Control newControl)
    {
        // Find the LayoutControl in the RootConnectedNode
        XmlControl? xmlControl = EditorController.RootConnectedNode.Find(layoutControl);
        if (xmlControl != null)
        {
            // Create a new Xml node but without the Properties and Inner Text. => This will be created in the Serializer.
            XmlElement newNode = xmlControl.Node.OwnerDocument.CreateElement(newControl.GetType().Name);
            
            xmlControl.Node.AppendChild(newNode);
            xmlControl.Children.Add(new XmlControl()
            {
                Control = newControl,
                Parent = xmlControl,
                Node = newNode,
            });
        }
    }
}