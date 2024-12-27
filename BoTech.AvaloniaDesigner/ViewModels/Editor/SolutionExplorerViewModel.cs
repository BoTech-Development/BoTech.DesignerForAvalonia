using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ReactiveUI;

namespace BoTech.AvaloniaDesigner.ViewModels.Editor;

public class SolutionExplorerViewModel : ViewModelBase
{
    private ObservableCollection<TreeViewNode> _treeViewNodes = new();
    public ObservableCollection<TreeViewNode> TreeViewNodes
    {
        get => _treeViewNodes; 
        set => this.RaiseAndSetIfChanged(ref _treeViewNodes, value);
    } 
    public TreeViewNode? SelectedItem { get; set; }
    private string _selectedFolder = "";
    public SolutionExplorerViewModel(string selectedPath)
    {
        TreeViewNode node = new TreeViewNode("Files", new DirectoryInfo(selectedPath));
        UpdateTreeView(selectedPath, node);
        TreeViewNodes.Add(node);
    }

    public void OnTreeViewNodeSelectedChanged()
    {
        if (SelectedItem != null)
        {
            if (SelectedItem.File != null)
            {
                switch (SelectedItem.File.Extension)
                {
                    case "axaml":
                        break;
                    case "axaml.cs":
                        break;
                    case "cs":
                        break;
                }
            }
        }
    }

    public void UpdateTreeView(string selectedFolder, TreeViewNode currentNode)
    {
        // Add all Directories to the 
        List<string> directories = Directory.EnumerateDirectories(selectedFolder).ToList();
        foreach (string directory in directories)
        {
            // Get the Name only:
            string directoryName = new DirectoryInfo(directory).Name;
            TreeViewNode node = new TreeViewNode(directoryName, new DirectoryInfo(directory));
            
            currentNode.Children.Add(node);
            // Go Recursive through all Directories.
            UpdateTreeView(directory, node);
        }
        // Add all Files:
        List<string> files = Directory.EnumerateFiles(selectedFolder).ToList();
        foreach (string file in files)
        {
            string fileName = new FileInfo(file).Name;
            TreeViewNode node = new TreeViewNode(fileName, new FileInfo(file));
            currentNode.Children.Add(node);
        }
    }
    public class TreeViewNode
    {
        public ObservableCollection<TreeViewNode> Children { get; } = new ObservableCollection<TreeViewNode>();
        public string Text { get; set; } 
        public DirectoryInfo? Directory { get; set; }
        public FileInfo? File { get; set; }
        // TODO: Add Icons ( file-edit-outline => For .axaml Files; file-code-outline => For .cs Files oder .axaml.cs; file-question-outline => File Type not defined; file-star-four-points => For Style Files;  folder;

        public TreeViewNode(string text, DirectoryInfo directory)
        {
            Text = text;
            Directory = directory;
        }
        public TreeViewNode(string text, FileInfo file)
        {
            Text = text;
            File = file;
        }
    }
}