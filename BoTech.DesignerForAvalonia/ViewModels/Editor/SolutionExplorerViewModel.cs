using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Reflection.Metadata;
using Avalonia.Controls;
using BoTech.DesignerForAvalonia.Controller.Editor;
using BoTech.DesignerForAvalonia.Models.Editor;
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
    /// <summary>
    /// Saves all visible files or directories
    /// </summary>
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
    /// <summary>
    /// Saves all Views in the Solution
    /// </summary>
    public ObservableCollection<DisplayableProjectView> ProjectViews
    {
        get => _projectViews; 
        set => this.RaiseAndSetIfChanged(ref _projectViews, value);
    } 
    /// <summary>
    /// Loads the Selected View
    /// </summary>
    public ReactiveCommand<string, Unit> EditViewCommand { get; }
    /// <summary>
    /// When button Pressed, the system will search for the string
    /// </summary>
    public ReactiveCommand<Unit, Unit> SearchCommand { get; set; }
    /// <summary>
    /// Show all Files or the Solution
    /// </summary>
    public ReactiveCommand<Unit, Unit> DeleteSearchResultsCommand { get; set; }
    /// <summary>
    /// The text which the user has entered int the search bar.
    /// </summary>
    public string CurrentSearchText { get; set; }
    public EditorController EditorController { get; set; }
   

    
    private bool _explorerType = true;
    private TreeViewNode _fileSystemTreeViewNode = null!;
    private TreeViewNode _solutionTreeViewNode = null!;
    
    private Deserializer? _deserializer = null;
    
    private Project _project;

    public SolutionExplorerViewModel(Project project , EditorController editorController, SolutionExplorerView codeBehind) : base(codeBehind)
    {
        EditorController = editorController;

        EditViewCommand = ReactiveCommand.Create<string>(EditView);

        SearchCommand = ReactiveCommand.Create(() =>
        {
            SearchForAFileOrFolder(CurrentSearchText);
        });
        DeleteSearchResultsCommand = ReactiveCommand.Create(() =>
        {
            OnExplorerTypeChanged(false);
        });
        
        _project = project;
        
        UpdateTreeView(project.SolutionFile, project.SolutionFilePath);
        SyncProjectByUsingTheFileTree(project, _fileSystemTreeViewNode);
        project.OutputPath = ExtractPathToAssemblyFromNodes(project.SolutionFilePath, Path.GetFileNameWithoutExtension(project.SolutionFilePath));
        try
        {
            project.Assembly = Assembly.LoadFile(path: project.OutputPath);
        }
        catch (Exception e)
        {
            // TODO: Better Error handling...
            Console.WriteLine("Error by loading the Project Assembly: " + e.ToString());
        }
        

      
       // Views.Clear();
        //Views = new ObservableCollection<DisplayableProjectView>(){ (IEnumerable<DisplayableProjectView>)Views.Concat(_project.Views) };

    }

    public void SearchForAFileOrFolder(string name)
    {
        if (_explorerType)
        {
            TreeViewNode searchResultNode = new TreeViewNode(_solutionTreeViewNode.Text, _solutionTreeViewNode.File);
            if (_solutionTreeViewNode.Search(name, searchResultNode))
            {
                TreeViewNodes.Clear();
                TreeViewNodes = new ObservableCollection<TreeViewNode>()
                {
                    (TreeViewNode)searchResultNode.Children.First()
                };
            }
            else
            {
                // Nothing found
                TreeViewNodes.Clear();
                TreeViewNodes = new ObservableCollection<TreeViewNode>()
                {
                    new TreeViewNode("Nothing Found.", _solutionTreeViewNode.File)
                };
            }
        }
        else
        {
            TreeViewNode searchResultNode = new TreeViewNode("Files", _fileSystemTreeViewNode.Directory);
            if (_fileSystemTreeViewNode.Search(name, searchResultNode))
            {
                TreeViewNodes.Clear();
                TreeViewNodes = new ObservableCollection<TreeViewNode>()
                {
                    (TreeViewNode)searchResultNode.Children.First()
                };
            }
            else
            {
                // Nothing found
                TreeViewNodes.Clear();
                TreeViewNodes = new ObservableCollection<TreeViewNode>()
                {
                    new TreeViewNode("Nothing Found.", _fileSystemTreeViewNode.Directory)
                };
            }
        }
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
    /// Change the View Type (Solution or File)
    /// </summary>
    /// <param name="changeType">Is only necessary for the Search Bar.</param>
    public void OnExplorerTypeChanged(bool changeType = true)
    {
        if(changeType)_explorerType = !_explorerType;
        if (_explorerType)
        {
            TreeViewNodes = new ObservableCollection<TreeViewNode>()
            {
                _solutionTreeViewNode
            };
        }
        else
        {
            TreeViewNodes = new ObservableCollection<TreeViewNode>()
            {
                _fileSystemTreeViewNode
            };
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
            if(filePath.Contains(projectName + "\\bin\\Debug\\net9.0\\")) return filePath;
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
        _solutionTreeViewNode = rootNode;
        TreeViewNodes = new ObservableCollection<TreeViewNode>(){_solutionTreeViewNode};
      
        TreeViewNode rootNode2 = new TreeViewNode("Files", new DirectoryInfo(selectedSolutionFile));
        if (file.Directory != null)
        {
            UpdateTreeViewFiles(file.Directory.FullName, rootNode2);
            _fileSystemTreeViewNode =  rootNode2;
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
                            view = project.Views.Where(view =>
                                    view.Name.Equals(rootFileNode.File.Name.Replace("View", "").Replace(".axaml", "")))
                                .FirstOrDefault();
                            if (view != null)
                            {
                                view.PathToView = rootFileNode.File.FullName;
                            }
                            else
                            {
                                project.Views.Add(new ProjectView()
                                {
                                    Name = rootFileNode.File.Name.Replace("View", "").Replace(".axaml", ""),
                                    PathToView = rootFileNode.File.FullName
                                });
                            }

                            break;
                        // It is the Code Behind
                        case ".axaml.cs":
                            view = project.Views.Where(view =>
                                    view.Name.Equals(rootFileNode.File.Name.Replace("View", "").Replace(".axaml.cs", "")))
                                .FirstOrDefault();
                            if (view != null)
                            {
                                view.PathToCodeBehind = rootFileNode.File.FullName;
                            }
                            else
                            {
                                project.Views.Add(new ProjectView()
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
                                view = project.Views.Where(view =>
                                    view.Name.Equals(rootFileNode.File.Name.Replace("ViewModel.cs", ""))).FirstOrDefault();
                                if (view != null)
                                {
                                    view.ViewModel = new ProjectViewModel
                                    {
                                        Path = rootFileNode.File.FullName,
                                        Name = "" // ADDED TO DEBUG
                                    };
                                    project.ViewModels.Add(view.ViewModel);
                                }
                                else
                                {
                                    ProjectViewModel viewModel = new ProjectViewModel
                                    {
                                        Path = rootFileNode.File.FullName,
                                        Name = null
                                    };
                                    project.Views.Add(new ProjectView()
                                    {
                                        Name = rootFileNode.File.Name.Replace("ViewModel.cs", ""),
                                        ViewModel = viewModel
                                    });
                                    project.ViewModels.Add(viewModel);
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
    public class TreeViewNode : TreeViewNodeBase
    {

        public DirectoryInfo? Directory { get; set; }
        public FileInfo? File { get; set; }
        public MaterialIconKind Icon { get; set; }
        
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
        protected override TreeViewNodeBase? Copy(TreeViewNodeBase nodeToCopy)
        {
            if (nodeToCopy is TreeViewNode treeViewNode)
            {
                if (treeViewNode.Directory != null) return new TreeViewNode(treeViewNode.Text, treeViewNode.Directory);
                if (treeViewNode.File != null) return new TreeViewNode(treeViewNode.Text, treeViewNode.File);
            }
            return null; // Error
        }
        /// <summary>
        /// Only for debugging purposes. 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
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