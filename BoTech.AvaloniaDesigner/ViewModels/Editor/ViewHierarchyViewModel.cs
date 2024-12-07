using System.Collections.ObjectModel;
using Avalonia.Controls;
using BoTech.AvaloniaDesigner.Controller.Editor;
using BoTech.AvaloniaDesigner.Services.Avalonia;
using DynamicData;

namespace BoTech.AvaloniaDesigner.ViewModels.Editor;

public class ViewHierarchyViewModel : ViewModelBase
{
    public ObservableCollection<TreeViewNode> TreeViewNodes { get; set; } 
    public TreeViewNode? SelectedItem { get; set; }
    
    private PreviewController _previewController;
    public ViewHierarchyViewModel(PreviewController previewController)
    {
        _previewController = previewController;
        _previewController.ViewHierarchyViewModel = this;
        TreeViewNodes = new ObservableCollection<TreeViewNode>();
    }

    public void OnTreeViewSelectionChanged()
    {
        
    }

    /// <summary>
    /// Method to update the TreeView, when the PreviewContent has changed. Will be called by the <see cref="PreviewController">
    ///     <cref>OnPreviewContentChanged</cref>
    /// </see>
    /// </summary>
    public void Reload()
    {
        TreeViewNode mainNode = GetTreeViewNodesFromControl(_previewController.PreviewContent);
        // Remove the main Node (Is useless)
        TreeViewNodes = mainNode.Children;
    }

    private TreeViewNode GetTreeViewNodesFromControl(Control control)
    {
        
        TreeViewNode newNode = new TreeViewNode()
        {
            Text = control.GetType().Name,
            ControlInstance = control,
        };
        // When the Control has Children
        Controls? nextControls = TypeCastingService.GetChildControlsOfLayoutControl(control);
        if (nextControls != null)
        {
            foreach (Control child in nextControls)
            {
                newNode.Children.Add(GetTreeViewNodesFromControl(child));
            }
        }
        // Add this Control 
        return newNode;
        
    }
    /// <summary>
    /// The TreeViewNode class is a model for each TreeView Node.<br/>
    /// It can be used to store the Text to display and an Instance for the referenced Control.
    /// </summary>
    public class TreeViewNode
    {
        public ObservableCollection<TreeViewNode> Children { get; } = new ObservableCollection<TreeViewNode>();
        public string Text { get; set; } = string.Empty;
        /// <summary>
        /// Referenced Control on the Preview 
        /// </summary>
        public required Control ControlInstance { get; set; } 
        // Icon
    }
}