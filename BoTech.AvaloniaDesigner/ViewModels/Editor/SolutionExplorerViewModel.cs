using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BoTech.AvaloniaDesigner.Controller.Editor;
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
    public EditorController EditorController { get; set; }
    private string _selectedFolder = "";
    private string _assemblyPath;
    private string _projectName;
    public SolutionExplorerViewModel(string projectName, string selectedPath, EditorController editorController)
    {
        EditorController = editorController;
        _projectName = projectName;
        TreeViewNode rootNode = new TreeViewNode("Files", new DirectoryInfo(selectedPath));
        UpdateTreeView(selectedPath, rootNode);
        TreeViewNodes.Add(rootNode);
        _assemblyPath = ExtractPathToAssemblyFromNodes(rootNode);
    }

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
                        break;
                    case ".axaml.cs":
                        LoadPreviewFromFile(SelectedItem.File.FullName, _assemblyPath);
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
       // XmlSerializer serializer = new XmlSerializer(typeof(Control));
       // StringReader reader = new StringReader(File.ReadAllText(pathToFile));
       
        //Control controla = (Control)serializer.Deserialize(reader);
           
        
        
        
        object newContent = AvaloniaRuntimeXamlLoader.Load(File.ReadAllText(pathToFile), Assembly.LoadFile(pathToAssembly));
        if (newContent is Control control)
        {
            EditorController.PreviewContent = new Grid()
            {
                Children =
                {
                    (Control)newContent
                }
            };
        }
        // RuntimeXamlLoaderDocument.
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