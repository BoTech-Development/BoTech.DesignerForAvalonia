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

namespace BoTech.AvaloniaDesigner.ViewModels.Editor;

public class ItemsExplorerViewModel : ViewModelBase
{
    public ObservableCollection<TreeViewNode> TreeViewNodes { get; set;  } 
    public TreeViewNode? SelectedItem { get; set; }
    
    // will be injected
    private PreviewController _previewController;
    
    public ItemsExplorerViewModel(PreviewController previewController)
    {
        _previewController = previewController;
      
       
        CreateTreeView();
    }

    private void CreateTreeView()
    {
        // Gets all Types which are nested under the Avalonia.? Namespace
        List<TypeInfo> allTypes = Assembly.Load(new AssemblyName("Avalonia.Controls")).DefinedTypes.ToList();
        // Therefore it is needed to get all Types which are directly nested under Avalonia.Controls
        List<TypeInfo> sortedTypes = allTypes.Where(type => type.Namespace == "Avalonia.Controls").ToList();
        
        // Because of in the new Sorted List are a lot of Types which we do not use for example Interfaces or Classes like Control, it is necessary to filter all Type which inherit from Control.
        List<TypeInfo> controlBasedTypes = sortedTypes.Where(type => type.AsType().IsSubclassOf(typeof(Control))).ToList();
        ObservableCollection<TreeViewNode> nodes = new ObservableCollection<TreeViewNode>();
        foreach (TypeInfo controlBasedType in controlBasedTypes)
        {
            nodes.Add(new TreeViewNode()
            {
                Text = controlBasedType.Name,
                ControlInstance = GetDefaultInstanceForControl(controlBasedType)
            });
        }

        TreeViewNodes = new ObservableCollection<TreeViewNode>()
        {
            new TreeViewNode()
            {
                Text = "Avalonia Controls",
                ControlInstance = null,
                Children = nodes
            }
        };
    }

    private Control GetDefaultInstanceForControl(Type controlType)
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
        if(SelectedItem != null) _previewController.StartDrag(SelectedItem.ControlInstance);
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
        public required Control ControlInstance { get; set; } 
        // Icon
    }
}