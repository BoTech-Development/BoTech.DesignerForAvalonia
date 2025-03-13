using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using BoTech.DesignerForAvalonia.Controller.Editor;
using BoTech.DesignerForAvalonia.Models.Editor;
using BoTech.DesignerForAvalonia.Services.Avalonia;
using BoTech.DesignerForAvalonia.ViewModels.Abstraction;
using BoTech.DesignerForAvalonia.Views.Editor;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.ViewModels.Editor;

public class ItemsExplorerViewModel : CloseablePageViewModel<ItemsExplorerView>
{
    private ObservableCollection<TreeViewNode> _treeViewNodes = new();
    
    /// <summary>
    /// Displayed Controls
    /// </summary>
    public ObservableCollection<TreeViewNode> TreeViewNodes 
    {
        get => _treeViewNodes; 
        set => this.RaiseAndSetIfChanged(ref _treeViewNodes, value);
    }  
    /// <summary>
    /// Selected Control
    /// </summary>
    public TreeViewNode? SelectedItem { get; set; }
    /// <summary>
    /// starts the search function
    /// </summary>
    public ReactiveCommand<Unit, Unit> SearchCommand { get; set; }
    /// <summary>
    /// Replaces the Search results to the original list of TreeViewNodes.
    /// </summary>
    public ReactiveCommand<Unit, Unit> DeleteSearchResultsCommand { get; set; }
    /// <summary>
    /// The search string that the User can enter.
    /// </summary>
    public string CurrentSearchText { get; set; }

    /// <summary>
    /// All Controls without the Search function Applied.
    /// </summary>
    private TreeViewNode _allControls;
    
    // will be injected
    private EditorController _editorController;
    
    public ItemsExplorerViewModel(EditorController editorController, ItemsExplorerView codeBehind) : base(codeBehind)
    {
        _editorController = editorController;
        SearchCommand =  ReactiveCommand.Create(Search);
        DeleteSearchResultsCommand = ReactiveCommand.Create(DeleteSearchResults);
        CreateTreeView();
        TreeViewNodes = new ObservableCollection<TreeViewNode>() { _allControls };
    }

    public void Search()
    {
        TreeViewNode searchNode = new TreeViewNode()
        {
            Text = "Avalonia Controls",
            ControlType = null,
        };
        if (_allControls.Search(CurrentSearchText, searchNode))
        {
            TreeViewNodes = new ObservableCollection<TreeViewNode>() { searchNode };
        }
        else
        {
            TreeViewNodes = new ObservableCollection<TreeViewNode>() 
            { 
                new TreeViewNode()
                {
                    Text = "Nothing Found.",
                    ControlType = null,
                } 
            };
        }
    }
    private void DeleteSearchResults() => TreeViewNodes = new ObservableCollection<TreeViewNode>() { _allControls };
    private void CreateTreeView()
    {
        List<TypeInfo> controlBasedTypes = TypeCastingService.GetAllControlBasedAvaloniaTypes();
        ObservableCollection<TreeViewNodeBase> nodes = new ObservableCollection<TreeViewNodeBase>();
        foreach (TypeInfo controlBasedType in controlBasedTypes)
        {
            nodes.Add(new TreeViewNode()
            {
                Text = controlBasedType.Name,
                ControlType = controlBasedType
            });
        }

        _allControls = new TreeViewNode()
        {
            Text = "Avalonia Controls",
            ControlType = null,
            Children = nodes
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
    public class TreeViewNode : TreeViewNodeBase
    {
       
        protected override TreeViewNodeBase? Copy(TreeViewNodeBase nodeToCopy)
        {
            TreeViewNode copy;
            if (nodeToCopy is TreeViewNode treeViewNode)
            {
                copy = new TreeViewNode()
                {
                    Text = treeViewNode.Text,
                    ControlType = treeViewNode.ControlType,
                    Count = treeViewNode.Count,
                };
                return copy;
            }
            return null;
        }

       
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