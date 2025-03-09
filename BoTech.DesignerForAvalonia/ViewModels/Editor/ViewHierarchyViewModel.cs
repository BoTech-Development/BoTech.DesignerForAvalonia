using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reflection;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using BoTech.DesignerForAvalonia.Controller.Editor;
using BoTech.DesignerForAvalonia.Models.XML;
using BoTech.DesignerForAvalonia.Services.Avalonia;
using BoTech.DesignerForAvalonia.ViewModels.Abstraction;
using BoTech.DesignerForAvalonia.Views.Editor;
using DynamicData;
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
    private bool _startMoveButtonVisible = true;
    public bool StartMoveButtonVisible
    {
        get => _startMoveButtonVisible; 
        set => this.RaiseAndSetIfChanged(ref _startMoveButtonVisible, value);
    } 
    
    public TreeViewNode? SelectedItem { get; set; }

    private TreeViewNode? _nodeToCopy;

    private TreeViewNode? _nodeToMove;
    
    private EditorController _editorController;
    public ViewHierarchyViewModel(EditorController editorController, ViewHierarchyView codeBehind) : base(codeBehind)
    {
        _editorController = editorController;
        _editorController.ViewHierarchyViewModel = this;
        TreeViewNodes = new ObservableCollection<TreeViewNode>();
        Reload();
    }
   
    public void StartMove(TreeViewNode node)
    {
        _nodeToMove = node;
    }

    public void StopMove(TreeViewNode node)
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

    public void Copy(TreeViewNode copyNode)
    {
        _nodeToCopy = copyNode;
    }

    public void Paste(TreeViewNode selectedNode)
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
    public void Delete(TreeViewNode selectedNode)
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
        TreeViewNode mainNode = GetTreeViewNodesFromControl(_editorController.PreviewContent);
        TreeViewNodes = new ObservableCollection<TreeViewNode>();
        TreeViewNodes.Add(mainNode);
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
    public class TreeViewNode
    {
        public ObservableCollection<TreeViewNode> Children { get; } = new ObservableCollection<TreeViewNode>();
        public string Text { get; set; } = string.Empty;
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
    }
}