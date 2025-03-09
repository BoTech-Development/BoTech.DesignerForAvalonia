using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Reflection.Metadata;
using Avalonia.Controls;
using BoTech.DesignerForAvalonia.Controller.Editor;
using BoTech.DesignerForAvalonia.Models.Project;
using BoTech.DesignerForAvalonia.Services.XML;
using BoTech.DesignerForAvalonia.ViewModels.Abstraction;
using BoTech.DesignerForAvalonia.Views.Editor;
using DynamicData;
using Material.Icons;
using Microsoft.Build.Construction;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.ViewModels.Editor;

public class SolutionExplorerViewModel : CloseablePageViewModel<SolutionExplorerView>
{
   
    /// <summary>
    /// Saves all visible files or directories
    /// </summary>
    private ObservableCollection<TreeViewNode> _treeViewNodes = new();
    public ObservableCollection<TreeViewNode> TreeViewNodes
    {
        get => _treeViewNodes; 
        set => this.RaiseAndSetIfChanged(ref _treeViewNodes, value);
    } 
    /// <summary>
    /// The Selected Item of the Tree View
    /// </summary>
    public TreeViewNode? SelectedItem { get; set; }
    /// <summary>
    /// Saves all Views in the Solution
    /// </summary>
    private ObservableCollection<DisplayableProjectView> _projectViews = new();
    public ObservableCollection<DisplayableProjectView> ProjectViews
    {
        get => _projectViews; 
        set => this.RaiseAndSetIfChanged(ref _projectViews, value);
    } 
    public ReactiveCommand<string, Unit> EditViewCommand { get; }
    
    public EditorController EditorController { get; set; }
   

    
    private bool _explorerType = true;
    private List<TreeViewNode> _fileSystemTreeViewNodes = new List<TreeViewNode>();
    private List<TreeViewNode> _solutionTreeViewNodes = new List<TreeViewNode>();
    
    private Deserializer? _deserializer = null;
    
    private Project _project;

    public SolutionExplorerViewModel(Project project , EditorController editorController, SolutionExplorerView codeBehind) : base(codeBehind)
    {
        EditorController = editorController;

        EditViewCommand = ReactiveCommand.Create<string>(EditView);
        _project = project;
        
        UpdateTreeView(project.SolutionFile, project.SolutionFilePath);
        SyncProjectByUsingTheFileTree(project, _fileSystemTreeViewNodes[0]);
        project.OutputPath = ExtractPathToAssemblyFromNodes(project.SolutionFilePath, Path.GetFileNameWithoutExtension(project.SolutionFilePath));

      
       // ProjectViews.Clear();
        //ProjectViews = new ObservableCollection<DisplayableProjectView>(){ (IEnumerable<DisplayableProjectView>)ProjectViews.Concat(_project.ProjectViews) };

    }
    
    /// <summary>
    /// Will be called by the View Menu under the treeView.
    /// </summary>
    /// <param name="viewName"></param>
    public void EditView(string viewName)
    {
        ProjectView? projectView = ProjectViews.FirstOrDefault(p => p.Name == viewName);
        if (projectView != null)
        {
            LoadPreviewFromFile(projectView.PathToView, _project.OutputPath);
        }
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
                        LoadPreviewFromFile(SelectedItem.File.FullName,  _project.OutputPath);
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
    /// Changes the View Type (Solution or File)
    /// </summary>
    public void OnExplorerTypeChanged()
    {
        _explorerType = !_explorerType;
        if (_explorerType)
        {
            TreeViewNodes = new ObservableCollection<TreeViewNode>(_solutionTreeViewNodes.ToList());
        }
        else
        {
            TreeViewNodes = new ObservableCollection<TreeViewNode>(_fileSystemTreeViewNodes.ToList());
        }
    }
    /// <summary>
    /// This Method Update the current PreviewContent Property in the EditorController to the new File that should be load.
    /// </summary>
    /// <param name="pathToFile">Full-path to the .axaml File</param>
    /// <param name="pathToAssembly">Full-path to the prebuilt Assembly of the loaded Project.</param>
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
    /// <param name="selectedSolutionFile">The Path to the selected SolutionFile</param>
    /// <param name="projectName">The Name of the Solution or Project</param>
    /// <returns>The full Path to the Assembly or string.Empty when the File was not found.</returns>
    private string ExtractPathToAssemblyFromNodes(string selectedSolutionFile, string projectName)
    {
        FileInfo fileInfo = new FileInfo(selectedSolutionFile);
       
        string[] possibleFiles = Directory.GetFiles(fileInfo.Directory.FullName, projectName + ".dll", SearchOption.AllDirectories);
        foreach (string filePath in possibleFiles)
        {
            if(filePath.Contains(projectName + "\\bin\\Debug\\net8.0\\")) return filePath;
        }
        return null;
    }
    /// <summary>
    /// Updates the SolutionExplorer by using the _explorerType Config var, which describes if the solution view should be loaded or the files view.
    /// </summary>
    /// <param name="solutionFile">The Selected solution File.</param>
    /// <param name="selectedSolutionFile">The Path of the selected Solution File.</param>
    public void UpdateTreeView(SolutionFile solutionFile, string selectedSolutionFile)
    {
      
        FileInfo file = new FileInfo(selectedSolutionFile);
        TreeViewNode rootNode = new TreeViewNode(file.Name.Replace(".sln", string.Empty), file);
        UpdateTreeViewSolution(solutionFile, rootNode);
        _solutionTreeViewNodes = new List<TreeViewNode> { rootNode };
        TreeViewNodes = new ObservableCollection<TreeViewNode>(_solutionTreeViewNodes);
      
        TreeViewNode rootNode2 = new TreeViewNode("Files", new DirectoryInfo(selectedSolutionFile));
        if (file.Directory != null)
        {
            UpdateTreeViewFiles(file.Directory.FullName, rootNode2);
            _fileSystemTreeViewNodes = new List<TreeViewNode>() { rootNode2 };
        }

    }
    /// <summary>
    /// Wrapper Method to execute UpdateTreeViewFiles.
    /// </summary>
    /// <param name="solutionFile"></param>
    /// <param name="rootNode"></param>
    private void UpdateTreeViewSolution(SolutionFile solutionFile, TreeViewNode rootNode)
    {
        foreach (ProjectInSolution project in solutionFile.ProjectsInOrder)
        {
            TreeViewNode projectNode = new TreeViewNode(project.ProjectName, new FileInfo(project.AbsolutePath));
            rootNode.Children.Add(projectNode);
            FileInfo projectFile = new FileInfo(project.AbsolutePath);
            if (projectFile.Directory != null)
            {
                // ProjectInSolution does not contain any Information about the Files which the Solution Contains.
                // Therefore, it is necessary to load the Files separately.
                UpdateTreeViewFiles(projectFile.Directory.FullName, projectNode, true);
            }
        }
    }
   
    /// <summary>
    /// Update the Tree which represents the Folder Structure.
    /// You can choose by using the values <see cref="excludeBinaryFolderOrFiles"/>, <see cref="excludeIdeOrGitFolderOrFiles"/>, <see cref="excludeProjectOrSolutionFiles"/>, which parts of the Loaded Directory should be visible or not.
    /// </summary>
    /// <param name="selectedFolder"></param>
    /// <param name="currentNode"></param>
    /// <param name="excludeBinaryFolderOrFiles"> When true, the Method will not inject Debug folder or .exe Files.</param>
    /// <param name="excludeIdeOrGitFolderOrFiles"> When true, the Method will not inject Ide Folders or Files like .idea, .vs or .git, to the File Tree View.</param>
    /// <param name="excludeProjectOrSolutionFiles"> When true, the Method will not inject Project or Solution Files, which ends with the name .csproj or .sln</param>
    private void UpdateTreeViewFiles(string selectedFolder, TreeViewNode currentNode, bool excludeBinaryFolderOrFiles = false, bool excludeIdeOrGitFolderOrFiles = false, bool excludeProjectOrSolutionFiles = false)
    {
        // Add all Directories to the 
        List<DirectoryInfo> directoryInfos = new DirectoryInfo(selectedFolder).EnumerateDirectories().ToList();
        foreach (DirectoryInfo directoryInfo in directoryInfos)
        {
            // Get the Name only:
            string directoryName = directoryInfo.Name;
            if(excludeBinaryFolderOrFiles && (directoryName.Contains("bin") || directoryName.Contains("obj"))) continue;
            if(excludeIdeOrGitFolderOrFiles && (directoryName.Contains(".git")|| directoryName.Contains(".github")|| directoryName.Contains(".idea")|| directoryName.Contains(".vs"))) continue;
            
            TreeViewNode node = new TreeViewNode(directoryName, directoryInfo);
            currentNode.Children.Add(node);
            // Go Recursive through all Directories.
            UpdateTreeViewFiles(directoryInfo.FullName, node, excludeBinaryFolderOrFiles);
          
        }
        // Add all Files:
        List<string> files = Directory.EnumerateFiles(selectedFolder).ToList();
        foreach (string file in files)
        {
            FileInfo fileInfo = new FileInfo(file);
            string fileName = fileInfo.Name;
            if (excludeProjectOrSolutionFiles && (fileInfo.Extension == ".csproj" || fileInfo.Extension == ".sln")) continue;
            
            TreeViewNode node = new TreeViewNode(fileName, new FileInfo(file));
            currentNode.Children.Add(node);
           
        }
    }
    /// <summary>
    /// Search for all Views which are Listed in the rootFileNode and adds them to the project Instance.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="rootFileNode"></param>
    private void SyncProjectByUsingTheFileTree(Project project, TreeViewNode rootFileNode)
    {
        if (rootFileNode.File != null)
        {
            if(!rootFileNode.File.Name.Contains("ViewModelBase") && !rootFileNode.File.Name.Contains("App"))
            {
                ProjectView? view = null;
                int indexOfDot = rootFileNode.File.Name.IndexOf('.');
                if (indexOfDot >= 0)
                {
                    switch (rootFileNode.File.Name.Substring(indexOfDot))
                    {
                        // It is a View
                        case ".axaml":
                            view = project.ProjectViews.Where(view =>
                                    view.Name.Equals(rootFileNode.File.Name.Replace("View", "").Replace(".axaml", "")))
                                .FirstOrDefault();
                            if (view != null)
                            {
                                view.PathToView = rootFileNode.File.FullName;
                            }
                            else
                            {
                                project.ProjectViews.Add(new ProjectView()
                                {
                                    Name = rootFileNode.File.Name.Replace("View", "").Replace(".axaml", ""),
                                    PathToView = rootFileNode.File.FullName
                                });
                            }

                            break;
                        // It is the Code Behind
                        case ".axaml.cs":
                            view = project.ProjectViews.Where(view =>
                                    view.Name.Equals(rootFileNode.File.Name.Replace("View", "").Replace(".axaml.cs", "")))
                                .FirstOrDefault();
                            if (view != null)
                            {
                                view.PathToCodeBehind = rootFileNode.File.FullName;
                            }
                            else
                            {
                                project.ProjectViews.Add(new ProjectView()
                                {
                                    Name = rootFileNode.File.Name.Replace("View", "").Replace(".axaml.cs", ""),
                                    PathToCodeBehind = rootFileNode.File.FullName
                                });
                            }

                            break;
                        // It can be an ViewModel
                        case ".cs":
                            if (rootFileNode.File.Name.Contains("ViewModel"))
                            {
                                view = project.ProjectViews.Where(view =>
                                    view.Name.Equals(rootFileNode.File.Name.Replace("ViewModel.cs", ""))).FirstOrDefault();
                                if (view != null)
                                {
                                    view.PathToViewModel = rootFileNode.File.FullName;
                                }
                                else
                                {
                                    project.ProjectViews.Add(new ProjectView()
                                    {
                                        Name = rootFileNode.File.Name.Replace("ViewModel.cs", ""),
                                        PathToViewModel = rootFileNode.File.FullName
                                    });
                                }
                            }

                            break;
                    }
                }
            }
        }
        foreach (TreeViewNode child in rootFileNode.Children)
        {
            SyncProjectByUsingTheFileTree(project, child);
        }
    }
    public class TreeViewNode
    {
        public ObservableCollection<TreeViewNode> Children { get; } = new ObservableCollection<TreeViewNode>();
        /// <summary>
        /// The Visible Text.
        /// </summary>
        public string Text { get; set; } 
        public DirectoryInfo? Directory { get; set; }
        public FileInfo? File { get; set; }
        
        
        public MaterialIconKind Icon { get; set; }
        // TODO: Add Icons ( file-edit-outline => For .axaml Files; file-code-outline => For .cs Files oder .axaml.cs; file-question-outline => File Type not defined; file-star-four-points => For Style Files;  folder;

        public TreeViewNode(string text, DirectoryInfo directory)
        {
            Text = text;
            Directory = directory;
            Icon = MaterialIconKind.Folder;
        }
        public TreeViewNode(string text, FileInfo file)
        {
            Text = text;
            File = file;
            switch (file.Extension)
            {
                case ".axaml":
                    Icon = MaterialIconKind.FileEditOutline;
                    break;
                case ".cs":
                    Icon = MaterialIconKind.FileCodeOutline;
                    break;
                case ".axaml.cs":
                    Icon = MaterialIconKind.FileCogOutline;
                    break;
                case ".csproj":
                    Icon = MaterialIconKind.CodeBlockBraces;
                    break;
                case ".sln":
                    Icon = MaterialIconKind.ApplicationBracketsOutline;
                    break;
                default:
                    Icon = MaterialIconKind.FileQuestionOutline;
                    break;
            }
        }
    }

    public class DisplayableProjectView : ProjectView
    {
        public ReactiveCommand<string, Unit> EditViewCommand { get; }
        private SolutionExplorerViewModel _vm;
        public DisplayableProjectView(SolutionExplorerViewModel vm) : base()
        {
            _vm = vm;
            EditViewCommand = ReactiveCommand.Create<string>(viewName => _vm.EditView(viewName));
        }
    }
}