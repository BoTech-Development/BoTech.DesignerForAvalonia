using System.Collections.ObjectModel;
using BoTech.DesignerForAvalonia.ViewModels.Editor;

namespace BoTech.DesignerForAvalonia.Models.Editor;
/// <summary>
/// Implements some basic functionality for all TreeViewNodes (SolutionExplorer, ViewHierarchy and ItemsExplorer)
/// </summary>
public abstract class TreeViewNodeBase
{
    /// <summary>
    /// Will be used by the Search Functionality.
    /// </summary>
    public string Text { get; set; }
    public ObservableCollection<TreeViewNodeBase> Children { get; set;  } = new ObservableCollection<TreeViewNodeBase>();
    
    /// <summary>
    /// This Method search for an Item in the current instance, where the Text Property Contains the given Text.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="newNode"></param>
    /// <returns>True when any Item was found, else false</returns>
    public bool Search(string text, TreeViewNodeBase newNode, bool firstIteration = true)
    {
        if (firstIteration)
        {
            bool foundInChild = false;
            foreach (TreeViewNodeBase child in Children)
                if (child.Search(text, newNode, false))
                    foundInChild = true;
            return foundInChild;
        }
        else
        {
            TreeViewNodeBase? copy = Copy(this);
            if (copy != null)
            {
                bool found = Text.Contains(text);
                bool foundInChild = false;
                foreach (TreeViewNodeBase child in Children)
                    if (child.Search(text, copy, false))
                        foundInChild = true;
                if (found || foundInChild)
                {
                    newNode.Children.Add(copy);
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Method should Copy the given Node and return the Copy.
    /// </summary>
    /// <param name="nodeToCopy"></param>
    /// <returns></returns>
    protected abstract TreeViewNodeBase? Copy(TreeViewNodeBase nodeToCopy);

}