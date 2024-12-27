using System.Collections.ObjectModel;
using Avalonia.Controls;
using BoTech.AvaloniaDesigner.Controller.Editor;
using BoTech.AvaloniaDesigner.Services.Avalonia;
using DynamicData;
using ReactiveUI;

namespace BoTech.AvaloniaDesigner.ViewModels.Editor;

public class ViewHierarchyViewModel : ViewModelBase
{
    private ObservableCollection<TreeViewNode> _treeViewNodes = new();
    public ObservableCollection<TreeViewNode> TreeViewNodes
    {
        get => _treeViewNodes; 
        set => this.RaiseAndSetIfChanged(ref _treeViewNodes, value);
    } 
    public TreeViewNode? SelectedItem { get; set; }
    
    private EditorController _editorController;
    public ViewHierarchyViewModel(EditorController editorController)
    {
        _editorController = editorController;
        _editorController.ViewHierarchyViewModel = this;
        TreeViewNodes = new ObservableCollection<TreeViewNode>();
        Reload();
    }

    public void OnTreeViewSelectionChanged()
    {
        if (SelectedItem != null)
        {
            _editorController.SelectedControl = SelectedItem.ControlInstance;
            _editorController.OnSelectedControlChanged();// Call the Event so that all other views will notified.
        }
    }

    /// <summary>
    /// Method to update the TreeView, when the PreviewContent has changed. Will be called by the <see cref="EditorController">
    ///     <cref>OnPreviewContentChanged</cref>
    /// </see>
    /// </summary>
    public void Reload()
    {
        TreeViewNode mainNode = GetTreeViewNodesFromControl(_editorController.PreviewContent);
        TreeViewNodes = new ObservableCollection<TreeViewNode>();
        TreeViewNodes.Add(mainNode);
        // Remove the main Node (Is useless)
        //TreeViewNodes = mainNode.Children;
    }

    private TreeViewNode GetTreeViewNodesFromControl(Control control)
    {
        
        TreeViewNode newNode = new TreeViewNode()
        {
            Text = control.GetType().Name + " (" + control.Name + ")",
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