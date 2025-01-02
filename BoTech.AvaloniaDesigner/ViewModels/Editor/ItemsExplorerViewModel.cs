using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using BoTech.AvaloniaDesigner.Controller.Editor;
using BoTech.AvaloniaDesigner.Services.Avalonia;

namespace BoTech.AvaloniaDesigner.ViewModels.Editor;

public class ItemsExplorerViewModel : ViewModelBase
{
    public ObservableCollection<TreeViewNode> TreeViewNodes { get; set;  } 
    public TreeViewNode? SelectedItem { get; set; }
    
    // will be injected
    private EditorController _editorController;
    
    public ItemsExplorerViewModel(EditorController editorController)
    {
        _editorController = editorController;
      
       
        CreateTreeView();
    }

    private void CreateTreeView()
    {
        List<TypeInfo> controlBasedTypes = TypeCastingService.GetAllControlBasedAvaloniaTypes();
        ObservableCollection<TreeViewNode> nodes = new ObservableCollection<TreeViewNode>();
        foreach (TypeInfo controlBasedType in controlBasedTypes)
        {
            nodes.Add(new TreeViewNode()
            {
                Text = controlBasedType.Name,
                ControlType = controlBasedType
            });
        }

        TreeViewNodes = new ObservableCollection<TreeViewNode>()
        {
            new TreeViewNode()
            {
                Text = "Avalonia Controls",
                ControlType = null,
                Children = nodes
            }
        };
    }
    /// <summary>
    /// Returns an instance of the given Control Type with a default Text or Content.
    /// Additionally, the Name of the Control will bes et with a count Var.
    /// </summary>
    /// <param name="controlType"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    private Control GetDefaultInstanceForControl(Type controlType, int count)
    {
        try
        {
            Control? control = Activator.CreateInstance(controlType) as Control;
            if (control != null)
            {
                if (controlType.GetProperty("Text") != null)
                {
                    // Control has the Text Property
                    control.GetType().GetProperty("Text").SetValue(control, "Your Text goes here...");
                }

                if (controlType.GetProperty("Content") != null)
                {
                    control.GetType().GetProperty("Content").SetValue(control, "Your Content goes here...");
                }
                
                // Create the unique Name:
                if (controlType.GetProperty("Name") != null)
                {
                    control.GetType().GetProperty("Name").SetValue(control, controlType.Name + "_" + count.ToString());
                }
                return control;
            }
            return new TextBlock()
            {
                Text = "Error by creating an Instance of Type: " + controlType.FullName + " dynamically created Type was null.",
                Foreground = Brushes.Orange,
            };
        }
        catch (Exception e)
        {
            return new TextBlock()
            {
                Text = "Error by creating an Instance of Type: " + controlType.FullName + "\n Error Message: " + e.Message,
                Foreground = Brushes.Orange,
            };
        }
    }
    
    
    /// <summary>
    /// This Method will be called from the Code behind when the User selects an Item of the Tree View.
    /// </summary>
    public void OnTreeViewSelectionChanged()
    {
        if (SelectedItem != null) //&& SelectedItem != _oldSelectedItem)
        {
            _editorController.StartDrag(GetDefaultInstanceForControl(SelectedItem.ControlType, SelectedItem.Count));
            SelectedItem.Count++;
        }
    }
    /// <summary>
    /// The TreeViewNode class is a model for each TreeView Node.<br/>
    /// It can be used to store the Text to display and an Instance for the referenced Control.
    /// </summary>
    public class TreeViewNode
    {
        public ObservableCollection<TreeViewNode> Children { get; set;  } = new ObservableCollection<TreeViewNode>();
        public string Text { get; set; } = string.Empty;
        /// <summary>
        /// Referenced Control
        /// </summary>
        public required Type ControlType { get; set; } 
        /// <summary>
        /// Counts how often the Control was added to the Preview to create a unique name for the Control.
        /// </summary>
        public int Count { get; set; }
        // Icon
    }
}