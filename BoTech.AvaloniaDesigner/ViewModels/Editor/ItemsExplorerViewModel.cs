using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalonia.Controls;
using BoTech.AvaloniaDesigner.Controller.Editor;

namespace BoTech.AvaloniaDesigner.ViewModels.Editor;

public class ItemsExplorerViewModel : ViewModelBase
{
    public ObservableCollection<TreeViewNode> TreeViewNodes { get; } 
    public TreeViewNode? SelectedItem { get; set; }
    
    // will be injected
    private DragAndDropController _dragAndDropController;
    
    public ItemsExplorerViewModel(DragAndDropController dragAndDropController)
    {
        _dragAndDropController = dragAndDropController;
        TreeViewNodes = new ObservableCollection<TreeViewNode>()
        {
            new TreeViewNode()
            {
                Text = "Avalonia Controls",
                ControlInstance = null,
                Children =
                {
                    new TreeViewNode()
                    {
                        ControlInstance = new Button()
                        {
                            Content = "Button",
                        },
                        Text = "Button",
                    },
                    new TreeViewNode()
                    {
                        ControlInstance = new TextBlock()
                        {
                            Text = "TextBlock",
                        },
                        Text = "TextBlock",
                    }
                }
            }
        };
    }
    
    /// <summary>
    /// This Method will be called from the Code behind when the User selects an Item of the Tree View.
    /// </summary>
    public void OnTreeViewSelectionChanged()
    {
        if(SelectedItem != null) _dragAndDropController.StartDrag(SelectedItem.ControlInstance);
    }
    /// <summary>
    /// The TreeViewNode class i an model for each TreeView Node.<br/>
    /// It can be used to store the Text to display and an Instance for the referenced Control.
    /// </summary>
    public class TreeViewNode
    {
        public ObservableCollection<TreeViewNode> Children { get; } = new ObservableCollection<TreeViewNode>();
        public string Text { get; set; } = string.Empty;
        /// <summary>
        /// Referenced Control
        /// </summary>
        public required Control ControlInstance { get; set; } 
        // Icon
    }
}