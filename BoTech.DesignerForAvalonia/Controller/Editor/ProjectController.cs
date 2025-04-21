
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using BoTech.DesignerForAvalonia.Models.Project;
using BoTech.DesignerForAvalonia.Services.CSharp;
using BoTech.DesignerForAvalonia.Services.PropertiesView;
using BoTech.DesignerForAvalonia.ViewModels;
using BoTech.DesignerForAvalonia.ViewModels.Editor;
using BoTech.DesignerForAvalonia.Views;
using DialogHostAvalonia;
using Microsoft.Build.Construction;

namespace BoTech.DesignerForAvalonia.Controller.Editor;

public class ProjectController
{
    /// <summary>
    /// Is needed to change the Content of the Main View
    /// </summary>
    private MainViewModel _mainViewModel;
    /// <summary>
    /// Saves all Recent Projects
    /// </summary>
    public List<Project> RecentProjects { get; private set; } = new List<Project>();
    /// <summary>
    /// Should be for all classes available.
    /// </summary>
    public EditorController EditorController { get; private set; }
    /// <summary>
    /// Should be available for all classes.
    /// </summary>
    public Project LoadedProject { get; private set; }
    
    private LoadingViewModel _loadingViewModel;
    private LoadingView _loadingView;
    public ProjectController(MainViewModel mainViewModel)
    {
        _loadingViewModel = new LoadingViewModel();
        _loadingView = new LoadingView()
        {
            DataContext = _loadingViewModel,
        };
        _mainViewModel = mainViewModel;
       
    }
    /// <summary>
    /// Loads all Recent Projects from the File
    /// </summary>
    public void Initialize()
    {
        LoadRecentProjectsFromFile();
    }

  
    /// <summary>
    /// Loads all Recent Porjects From file and inserts them sorted into the DisplayedProject List
    /// </summary>
    /// <exception cref="FormatException">Occurs when the file does not have the correct Format.</exception>
    private void LoadRecentProjectsFromFile()
    {
        if (File.Exists(Environment.CurrentDirectory + "\\RecentProjects.json"))
        {
            RecentProjects = JsonSerializer.Deserialize<List<Project>>(File.ReadAllText(Environment.CurrentDirectory + "\\RecentProjects.json"));
            if (RecentProjects != null)
            {
                RecentProjects.Sort((x, y) => x.LastUsed.CompareTo(y.LastUsed));
                //DisplayedProjects = new ObservableCollection<OpenableProject>();
                List<Project> projectsToDelete = new List<Project>();
                // Transfer Model to the ItemsControl and loading the SolutionFile.
                foreach (Project recentProject in RecentProjects)
                {
                    // When the Project does not exist
                    if (!File.Exists(recentProject.SolutionFilePath))
                    {
                        projectsToDelete.Add(recentProject);
                    }
                    else
                    {
                        recentProject.SolutionFile = SolutionFile.Parse(recentProject.SolutionFilePath);
                        // DisplayedProjects.Add(new OpenableProject(this, recentProject));
                    }
                }
                // Deleting all Projects which aren't exists
                foreach (Project projectToDelete in projectsToDelete) RecentProjects.Remove(projectToDelete);
            }
            else
            {
                throw new FormatException("Recent Projects file found (\"" + Environment.CurrentDirectory + "\\RecentProjects.json\"), but has the wrong file Format.");
            }
        }
        else
        {
            // Create and Save File if it does not exist.
            StreamWriter writer = new StreamWriter(Environment.CurrentDirectory + "\\RecentProjects.json");
            RecentProjects = new List<Project>();
            //DisplayedProjects = new ObservableCollection<OpenableProject>();
            writer.Write(JsonSerializer.Serialize(RecentProjects));
            writer.Close();
        }
    }
    /// <summary>
    /// Saves the current List of recent Projects
    /// </summary>
    private void SaveRecentProjectsToFile()
    {
        if (!File.Exists(Environment.CurrentDirectory + "\\RecentProjects.json"))File.Create(Environment.CurrentDirectory + "\\RecentProjects.json");
        RecentProjects.Sort((x,y) => x.LastUsed.CompareTo(y.LastUsed));
        string text = JsonSerializer.Serialize<List<Project>>(RecentProjects);
        File.WriteAllText(Environment.CurrentDirectory + "\\RecentProjects.json", text);
    }
    

    /// <summary>
    /// Can Load a Project from an .sln File. Will be called by the Button Command which is located on the Main Page. Additionally, each displayed RecentProject can call this Method through an Command.
    /// </summary>
    public void LoadProject(string path, string absolutePath, string name)
    {
        _loadingViewModel.StatusText = "Loading Solution...";
        _loadingViewModel.SubStatusText = "Parsing Solution file: " + absolutePath;
        DialogHost.Show(_loadingView, "MainDialogHost");
        
        
        SolutionFile solutionFile = SolutionFile.Parse(path);
        Project? project = RecentProjects.Find(p => p.SolutionFilePath == path);
        if (project == null)
        {
            // Important (for the old and new Project):
            // The ViewModelPath, the ViewPath and all Views in the Project will be added by the SolutionExplorerViewModel,
            // because he knows all the files and folders in the project.
          
            _loadingViewModel.SubStatusText = "Creating new Project...";
            project = new Models.Project.Project()
            {
                Name = name,
                LastUsed = DateTime.Now,
                SolutionFilePath = absolutePath,
                SolutionFile = solutionFile,
            };
            string shortName = "";
            foreach (char letter in name.Where(c => Char.IsUpper(c) || !Char.IsLetterOrDigit(c)))
            {
                if(letter != '.')shortName += letter;
            }
            project.ShortName = shortName;
            project.DisplayableProjectInfo.SetColorByName("Blue");
            RecentProjects.Add(project);
            //DisplayedProjects.Add(new OpenableProject(this, project));
        }
        LoadProject(project);
    }
    /// <summary>
    /// Final Step of the Loading Process.
    /// Important (for the old and new Project):
    /// The ViewModelPath, the ViewPath and all Views in the Project will be added by the SolutionExplorerViewModel,
    /// because he knows all the files and folders in the project.
    /// </summary>
    /// <param name="project">The project, which should be loaded</param>
    public void LoadProject(Project project)
    {
        DialogHost.Show(_loadingView, "MainDialogHost");
        _loadingViewModel.SubStatusText = "Final loading steps...";
        LoadedProject = project;
        // The Editor Controller initialize all Views of the Editor.
        // Through the SolutionViewModel the project will add all Views and ViewModels to the Project Model. 
        EditorController = new EditorController();
        _mainViewModel.Content = EditorController.Init(project);
        ControlsCreator.EditorController = EditorController;
        
        // Now it is necessary to parse all ViewModels

        foreach (ProjectViewModel viewModel in project.ViewModels)
        {
            viewModel.ClassInfoFromFile = CSharpParser.GetClassesInfoFromFile(viewModel.Path);
            if (viewModel.ClassInfoFromFile == null) { viewModel.IsAssemblyEqualsToSource = false; continue; }
            viewModel.IsAssemblyEqualsToSource = CSharpParser.IsClassInfoCompletelyImplementedInAssembly(project.Assembly, viewModel.ClassInfoFromFile);
        }
        // Now it is necessary to parse all Views (Code Behinds)
        foreach (ProjectView view in project.Views)
        {
            view.ClassInfoFromFile = CSharpParser.GetClassesInfoFromFile(view.PathToCodeBehind);
            if (view.ClassInfoFromFile == null) { view.IsAssemblyEqualsToSource = false; continue; }
            view.IsAssemblyEqualsToSource = CSharpParser.IsClassInfoCompletelyImplementedInAssembly(project.Assembly, view.ClassInfoFromFile);
        }

        
        
        // Saving the Editor Controller in the Top Navigation View to open views again.
        if (_mainViewModel.TopNavigationView.DataContext is TopNavigationViewModel topNavigationViewModel)
            topNavigationViewModel.OnProjectLoaded(this);
        
        _loadingViewModel.SubStatusText = "Saving Settings...";
        SaveRecentProjectsToFile();
        DialogHost.Close("MainDialogHost");
    }
    
}