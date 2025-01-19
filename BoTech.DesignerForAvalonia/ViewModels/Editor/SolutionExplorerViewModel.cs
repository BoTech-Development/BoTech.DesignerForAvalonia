using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using Avalonia.Controls;
using BoTech.DesignerForAvalonia.Controller.Editor;
using BoTech.DesignerForAvalonia.Services.XML;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.ViewModels.Editor;

public class SolutionExplorerViewModel : ViewModelBase
{
    private ObservableCollection<TreeViewNode> _treeViewNodes = new();
    public ObservableCollection<TreeViewNode> TreeViewNodes
    {
        get => _treeViewNodes; 
        set => this.RaiseAndSetIfChanged(ref _treeViewNodes, value);
    } 
    public TreeViewNode? SelectedItem { get; set; }
    public EditorController EditorController { get; set; }
   
    private string _assemblyPath;
    private string _projectName;

    
    private Deserializer? _deserializer = null;

    public SolutionExplorerViewModel(string projectName, string selectedPath, EditorController editorController)
    {
        EditorController = editorController;
        _projectName = projectName;
        TreeViewNode rootNode = new TreeViewNode("Files", new DirectoryInfo(selectedPath));
        UpdateTreeView(selectedPath, rootNode);
        TreeViewNodes.Add(rootNode);
        _assemblyPath = ExtractPathToAssemblyFromNodes(rootNode);
    }


    /// <summary>
    /// Gets called when the user Clicks on a File in the TreeView
    /// </summary>
    public void OnTreeViewNodeSelectedChanged()
    {
        if (SelectedItem != null)
        {
            if (SelectedItem.File != null)
            {
                switch (SelectedItem.File.Name.Substring(SelectedItem.File.Name.IndexOf('.'), SelectedItem.File.Name.Length - SelectedItem.File.Name.IndexOf('.')))
                {
                    case ".axaml":
                        LoadPreviewFromFile(SelectedItem.File.FullName, _assemblyPath);
                        EditorController.OnPreviewContentChanged();
                        EditorController.OpenedFilePath = SelectedItem.File.FullName;
                        break;
                    case ".axaml.cs":
                      //  LoadPreviewFromFile(SelectedItem.File.FullName, _assemblyPath);
                        break;
                    case ".cs":
                        break;
                }
            }
        }
    }
    /// <summary>
    /// This Method Update the current PreviewContent Property in the EditorController to the new File that should be load.
    /// </summary>
    /// <param name="pathToFile">Full-path to the .axaml File</param>
    /// <param name="pathToAssembly">Full-path to the builded Assembly of the loaded Project.</param>
    private void LoadPreviewFromFile(string pathToFile, string pathToAssembly)
    {
        if (File.Exists(pathToFile))
        {
            if(_deserializer == null) _deserializer = new Deserializer();
            //Extract Controls which are embedded in the UserControl or Window
            Control view = _deserializer.Deserialize(File.ReadAllText(pathToFile), Assembly.LoadFile(pathToAssembly));
            EditorController.RootConnectedNode = _deserializer.RootConnectedNode;
            
            Control? content = null;
            
            UserControl? userControl = view as UserControl;
            if (userControl != null)
            {
                content = userControl.Content as Control;
            }
            else
            {
                Window? window = view as Window;
                if (window != null)
                {
                    content = window.Content as Control;
                }
            }
            if (content != null)
                EditorController.PreviewContent = content;
        }
    }
    /// <summary>
    /// This Method searches in all subdirectories to find the pre-built File: For example: MyApp.Desktop.dll
    /// This Method can only search for an Assembly which was built for a Desktop device.
    /// </summary>
    /// <param name="rootNode"></param>
    /// <returns>The full Path to the Assembly or string.Empty when the File was not found.</returns>
    private string ExtractPathToAssemblyFromNodes(TreeViewNode rootNode)
    {
        if (rootNode.Text == _projectName + ".dll")
        {
            if(rootNode.File != null)
                return rootNode.File.FullName;
        }
        else
        {
            string path = string.Empty;
            foreach (TreeViewNode childNode in rootNode.Children)
            {
                path = ExtractPathToAssemblyFromNodes(childNode);
                if(path != string.Empty) return path;
            }
        }
        return string.Empty;
    }
    /// <summary>
    /// Update the Tree which represents the Folder Structure.
    /// </summary>
    /// <param name="selectedFolder"></param>
    /// <param name="currentNode"></param>
    private void UpdateTreeView(string selectedFolder, TreeViewNode currentNode)
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