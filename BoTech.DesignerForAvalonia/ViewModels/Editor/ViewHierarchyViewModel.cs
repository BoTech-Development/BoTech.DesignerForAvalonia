using System.Collections.ObjectModel;
using System.Reactive;
using System.Reflection;
using Avalonia.Controls;
using BoTech.DesignerForAvalonia.Controller.Editor;
using BoTech.DesignerForAvalonia.Models.Editor;
using BoTech.DesignerForAvalonia.Models.XML;
using BoTech.DesignerForAvalonia.Services.Avalonia;
using BoTech.DesignerForAvalonia.ViewModels.Abstraction;
using BoTech.DesignerForAvalonia.Views.Editor;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.ViewModels.Editor;

public class ViewHierarchyViewModel : CloseablePageViewModel<ViewHierarchyView>
{
    private ObservableCollection<TreeViewNode> _treeViewNodes = new();
    public ObservableCollection<TreeViewNode> TreeViewNodes
    {
        get => _treeViewNodes; 
        set => this.RaiseAndSetIfChanged(ref _treeViewNodes, value);
    } 
    /// <summary>
    /// Is true when the context menu should display the Move or Move here option.
    /// </summary>
    private bool _startMoveButtonVisible = true;
    public bool StartMoveButtonVisible
    {
        get => _startMoveButtonVisible; 
        set => this.RaiseAndSetIfChanged(ref _startMoveButtonVisible, value);
    } 
    /// <summary>
    /// The selected Node of the TreeView.
    /// </summary>
    public TreeViewNode? SelectedItem { get; set; }
    /// <summary>
    /// Node which was selected by the user to copy.
    /// </summary>
    private TreeViewNode? _nodeToCopy;
    /// <summary>
    /// Node which was selected by the user to move.
    /// </summary>
    private TreeViewNode? _nodeToMove;
    /// <summary>
    /// Starts the search function
    /// </summary>
    public ReactiveCommand<Unit, Unit> SearchCommand { get; set; }
    /// <summary>
    /// Deletes all search Results
    /// </summary>
    public ReactiveCommand<Unit, Unit> DeleteSearchResultsCommand { get; set; }
    /// <summary>
    /// The text content of the Search bar.
    /// </summary>
    public string CurrentSearchText { get; set; }
    /// <summary>
    /// The Main node of the TreeView must be stored separately, because the Property TreeViewNodes can be replaced with the search Results.
    /// </summary>
    private TreeViewNode _mainNode { get; set; }
    
    private EditorController _editorController;
    public ViewHierarchyViewModel(EditorController editorController, ViewHierarchyView codeBehind) : base(codeBehind)
    {
        _editorController = editorController;
        _editorController.ViewHierarchyViewModel = this;
        TreeViewNodes = new ObservableCollection<TreeViewNode>();
        SearchCommand = ReactiveCommand.Create(Search);
        DeleteSearchResultsCommand = ReactiveCommand.Create(DeleteSearchResults);
        Reload();
    }

    private void DeleteSearchResults() => TreeViewNodes = new ObservableCollection<TreeViewNode>() { _mainNode };
    public void Search()
    {
        TreeViewNode searchNode = new TreeViewNode(this)
        {
            Text = "Avalonia Controls",
            ControlInstance = null,
        };
        if (_mainNode.Search(CurrentSearchText, searchNode))
        {
            TreeViewNodes = new ObservableCollection<TreeViewNode>() { searchNode };
        }
        else
        {
            TreeViewNodes = new ObservableCollection<TreeViewNode>() 
            { 
                new TreeViewNode(this)
                {
                    Text = "Nothing Found.",
                    ControlInstance = null,
                } 
            };
        }
    }
    private void StartMove(TreeViewNode node)
    {
        _nodeToMove = node;
    }

    private void StopMove(TreeViewNode node)
    {
        if (_nodeToMove != null)
        {
            XmlControl? movedXmlControl = _editorController.RootConnectedNode.Find(_nodeToMove.ControlInstance);
            XmlControl? currentXmlControl = _editorController.RootConnectedNode.Find(node.ControlInstance);
            if (currentXmlControl != null && movedXmlControl != null)
            {
                XmlControl movedXMLControlClone = (XmlControl)movedXmlControl.Clone();
                movedXMLControlClone.Parent = currentXmlControl;
                // Creating new References
                currentXmlControl.Children.Add(movedXMLControlClone);    
                currentXmlControl.Node.AppendChild(movedXMLControlClone.Node);
                // Remove the all old references
                XmlControl? parent = movedXmlControl.Parent;
                if (parent != null)
                {
                    parent.Children.Remove(movedXmlControl);
                    parent.Node.RemoveChild(movedXmlControl.Node);
                    // Removing the moved Control from the Parent Control
                    PropertyInfo? propertyInfoParent;
                    if ((propertyInfoParent = parent.Control.GetType().GetProperty("Child")) != null)
                    {
                        propertyInfoParent.SetValue(parent.Control, null);
                    }
                    if ((propertyInfoParent = parent.Control.GetType().GetProperty("Content")) != null)
                    {
                        propertyInfoParent.SetValue(parent.Control, null);
                    }
                    if ((propertyInfoParent = parent.Control.GetType().GetProperty("Children")) != null)
                    {
                        Controls? children = propertyInfoParent.GetValue(parent.Control) as Controls;
                        if(children != null) children.Remove(movedXmlControl.Control);
                    }
                }
                
                // Update the Parent Control:
                // Adding the new created Control as an child Control of the parent control.
                
                PropertyInfo? propertyInfo;
                if ((propertyInfo = currentXmlControl.Control.GetType().GetProperty("Child")) != null)
                {
                    propertyInfo.SetValue(currentXmlControl.Control, movedXMLControlClone.Control);
                }
                if ((propertyInfo = currentXmlControl.Control.GetType().GetProperty("Content")) != null)
                {
                    propertyInfo.SetValue(currentXmlControl.Control, movedXMLControlClone.Control);
                }
                if ((propertyInfo = currentXmlControl.Control.GetType().GetProperty("Children")) != null)
                {
                    Controls? children = propertyInfo.GetValue(currentXmlControl.Control) as Controls;
                    if(children != null) children.Add(movedXMLControlClone.Control);
                }
                // Update the View
                Reload();
            }
        }
    }

    private void Copy(TreeViewNode copyNode)
    {
        _nodeToCopy = copyNode;
    }

    private void Paste(TreeViewNode selectedNode)
    {
        if (_nodeToCopy != null)
        {
            XmlControl? selectedXmlControl = _editorController.RootConnectedNode.Find(selectedNode.ControlInstance);
            XmlControl? xmlControl = _editorController.RootConnectedNode.Find(_nodeToCopy.ControlInstance);
            if (xmlControl != null && selectedXmlControl != null)
            {
                // XmlControl xmlControlCopy = (XmlControl)TypeCastingService.DeepCopyObject(xmlControl);
                XmlControl xmlControlCopy = (XmlControl)xmlControl.Clone();
                xmlControlCopy.Parent = selectedXmlControl;
                selectedXmlControl.Children.Add(xmlControlCopy);
                selectedXmlControl.Node.AppendChild(xmlControlCopy.Node);
                // Update the Parent Control:
                
                PropertyInfo? propertyInfo;
                if ((propertyInfo = selectedXmlControl.Control.GetType().GetProperty("Child")) != null)
                {
                    propertyInfo.SetValue(xmlControl, xmlControlCopy.Control);
                }
                if ((propertyInfo = selectedXmlControl.Control.GetType().GetProperty("Content")) != null)
                {
                    propertyInfo.SetValue(xmlControl, xmlControlCopy.Control);
                }
                if ((propertyInfo = selectedXmlControl.Control.GetType().GetProperty("Children")) != null)
                {
                    Controls? children = propertyInfo.GetValue(selectedXmlControl.Control) as Controls;
                    if(children != null) children.Add(xmlControlCopy.Control);
                }
                
                Reload();
            }
        }
    }
    private void Delete(TreeViewNode selectedNode)
    {
        XmlControl? xmlControl = _editorController.RootConnectedNode.Find(selectedNode.ControlInstance);
        if (xmlControl != null)
        {
            XmlControl? parentXmlControl = xmlControl.Parent;
            if (parentXmlControl != null)
            {
                // Remove the Control from the Parent Control.
                PropertyInfo? propertyInfo;
                if ((propertyInfo = parentXmlControl.Control.GetType().GetProperty("Child")) != null)
                {
                    propertyInfo.SetValue(xmlControl, null);
                }
                if ((propertyInfo = parentXmlControl.Control.GetType().GetProperty("Content")) != null)
                {
                    propertyInfo.SetValue(xmlControl, null);
                }
                if ((propertyInfo = parentXmlControl.Control.GetType().GetProperty("Children")) != null)
                {
                    Controls? children = propertyInfo.GetValue(parentXmlControl.Control) as Controls;
                    if(children != null) children.Remove(xmlControl.Control);
                }
            }
            // Delete the XmlNode from the Parent XmlNode 
            xmlControl.Parent?.Node.RemoveChild(xmlControl.Node);
            // Delete the XmlControl
            xmlControl.Parent?.Children.Remove(xmlControl);
            Reload();
        }
    }
    
    public void OnTreeViewSelectionChanged()
    {
        if (SelectedItem != null)
        {
            _editorController.SelectedControl = SelectedItem.ControlInstance;
            _editorController.OnSelectedControlChanged();// Call the Event so that all other views will be notified.
        }
    }

    /// <summary>
    /// Method to update the TreeView, when the PreviewContent has changed. Will be called by the <see cref="EditorController">
    ///     <cref>OnPreviewContentChanged</cref>
    /// </see>
    /// </summary>
    public void Reload()
    {
        _mainNode = GetTreeViewNodesFromControl(_editorController.PreviewContent);
        TreeViewNodes = new ObservableCollection<TreeViewNode>();
        TreeViewNodes.Add(_mainNode);
        // Remove the main Node (Is useless)
        //TreeViewNodes = mainNode.Children;
    }

    private TreeViewNode GetTreeViewNodesFromControl(Control control)
    {
        
        TreeViewNode newNode = new TreeViewNode(this)
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
    public class TreeViewNode : TreeViewNodeBase
    {
      
        /// <summary>
        /// Referenced Control on the Preview 
        /// </summary>
        public required Control ControlInstance { get; set; } 
        // Icon
        public ReactiveCommand<Unit, Unit> CopyCommand { get; set; }
        public ReactiveCommand<Unit, Unit> PasteCommand { get; set; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; set; }

        public ReactiveCommand<Unit, Unit> StartMoveCommand { get; set; }
        public ReactiveCommand<Unit, Unit> StopMoveCommand { get; set; }
        
        public ViewHierarchyViewModel ViewModel { get; set; }
        public TreeViewNode(ViewHierarchyViewModel viewHierarchyViewModel)
        {
            ViewModel = viewHierarchyViewModel;
            CopyCommand = ReactiveCommand.Create(() =>
            {
                ViewModel.Copy(this);
            });
            PasteCommand = ReactiveCommand.Create(() =>
            {
                ViewModel.Paste(this);
            });
            DeleteCommand = ReactiveCommand.Create(() =>
            {
                ViewModel.Delete(this);
            });
            StartMoveCommand = ReactiveCommand.Create(() =>
            {
                ViewModel.StartMove(this);
                ViewModel.StartMoveButtonVisible = false;
            });
            StopMoveCommand = ReactiveCommand.Create(() =>
            {
                ViewModel.StopMove(this);
                ViewModel.StartMoveButtonVisible = true;
            });
        }

        protected override TreeViewNodeBase? Copy(TreeViewNodeBase nodeToCopy)
        {
            if(nodeToCopy is TreeViewNode treeViewNode) return new TreeViewNode(treeViewNode.ViewModel)
            {
                Children = treeViewNode.Children,
                Text = treeViewNode.Text,
                ControlInstance = treeViewNode.ControlInstance,
            };
            return null;
        }
    }
}