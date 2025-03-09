using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using BoTech.DesignerForAvalonia.Controller.Editor;
using BoTech.DesignerForAvalonia.Models.Project;
using BoTech.DesignerForAvalonia.Services.PropertiesView;
using BoTech.DesignerForAvalonia.ViewModels.Editor;
using BoTech.DesignerForAvalonia.Views;
using Microsoft.Build.Construction;

using ReactiveUI;

namespace BoTech.DesignerForAvalonia.ViewModels;

public class ProjectStartViewModel : ViewModelBase
{
    private ObservableCollection<OpenableProject> _displayedProjects;

    public ObservableCollection<OpenableProject> DisplayedProjects
    {
        get => _displayedProjects;
        set => this.RaiseAndSetIfChanged(ref _displayedProjects, value);
    }
    public ReactiveCommand<Unit, Unit> OpenProjectCommand { get; set; }
    public ReactiveCommand<Unit, Unit> LoadProjectCommand { get; set; }

    
    private MainViewModel _mainViewModel;
    
    private bool _openFilePickerSuccess = false;
    private IStorageFile? currentSolutionFile = null;
    private List<Project> _recentProjects = new List<Project>();
    
    public ProjectStartViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        LoadProjectCommand = ReactiveCommand.Create(()=>
        {
            if (_openFilePickerSuccess && currentSolutionFile != null) LoadProject(currentSolutionFile.Path.LocalPath, currentSolutionFile.Path.AbsolutePath, currentSolutionFile.Name);
        });
        OpenProjectCommand = ReactiveCommand.CreateRunInBackground(OpenProject);
       
    
        LoadRecentProjectsFromFile();
        Project testProjectInfo = new Project()
        {
            Name = "TestProject",
            SolutionFilePath = @"c:\\temp\\test.sln",
            ShortName = "TP"
        };
        testProjectInfo.DisplayableProjectInfo.SetColorByName("Blue");
        DisplayedProjects.Add(new OpenableProject(this, testProjectInfo));
    }
    /// <summary>
    /// Loads all Recent Porjects From file and inserts them sorted into the DisplayedProject List
    /// </summary>
    /// <exception cref="FormatException">Occurs when the file does not have the correct Format.</exception>
    public void LoadRecentProjectsFromFile()
    {
        if (File.Exists(Environment.CurrentDirectory + "\\RecentProjects.json"))
        {
            _recentProjects = JsonSerializer.Deserialize<List<Project>>(File.ReadAllText(Environment.CurrentDirectory + "\\RecentProjects.json"));
            if (_recentProjects != null)
            {
                _recentProjects.Sort((x,y) => x.LastUsed.CompareTo(y.LastUsed));
                DisplayedProjects = new ObservableCollection<OpenableProject>();
                // Transfer Model to the ItemsControl and loading the SolutionFile.
                foreach (Project recentProject in _recentProjects)
                {
                    recentProject.SolutionFile = SolutionFile.Parse(recentProject.SolutionFilePath);
                    DisplayedProjects.Add(new OpenableProject(this, recentProject));
                }
            }
            else
            {
                throw new FormatException("Recent Projects file found (\"" + Environment.CurrentDirectory + "\\RecentProjects.json\"), but has the wrong file Format.");
            }
        }
        else
        {
            StreamWriter writer = new StreamWriter(Environment.CurrentDirectory + "\\RecentProjects.json");
            _recentProjects = new List<Project>();
            DisplayedProjects = new ObservableCollection<OpenableProject>();
            writer.Write(JsonSerializer.Serialize(_recentProjects));
            writer.Close();
        }
    }
    /// <summary>
    /// Saves the current List of recent Projects
    /// </summary>
    public void SaveRecentProjectsToFile()
    {
        if (!File.Exists(Environment.CurrentDirectory + "\\RecentProjects.json"))File.Create(Environment.CurrentDirectory + "\\RecentProjects.json");
        _recentProjects.Sort((x,y) => x.LastUsed.CompareTo(y.LastUsed));
        string text = JsonSerializer.Serialize<List<Project>>(_recentProjects);
        File.WriteAllText(Environment.CurrentDirectory + "\\RecentProjects.json", text);
    }
    
    /// <summary>
    /// This Method will be invoked by a Button on the UI, to show the OpenFilePicker, where the User can select an .sln File.
    /// It also will Load the Project and show the Working Screen after that.
    /// </summary>
    public async void OpenProject()
    {
        _openFilePickerSuccess = false;
        currentSolutionFile = await DoOpenFilePickerAsync();
        if (currentSolutionFile != null)
        {
            if (File.Exists(currentSolutionFile.Path.LocalPath))
            {
                _openFilePickerSuccess = true;
            }
        }
    }
    /// <summary>
    /// Can Load a Project from an .sln File.
    /// </summary>
    private void LoadProject(string path, string absolutePath, string name)
    {
   
        SolutionFile solutionFile = SolutionFile.Parse(path);
        Project? project = _recentProjects.Find(p => p.SolutionFilePath == path);
        if (project == null)
        {
            // Important (for the old and new Project):
            // The ViewModelPath, the ViewPath and all Views in the Project will be added by the SolutionExplorerViewModel,
            // because he knows all the files and folders in the project.
            project = new Project()
            {
                Name = name,
                ShortName = name.Where(c => Char.IsUpper(c) || !Char.IsLetterOrDigit(c)).ToString(),
                LastUsed = DateTime.Now,
                SolutionFilePath = absolutePath,
                SolutionFile = solutionFile,
            };
            project.DisplayableProjectInfo.SetColorByName("Blue");
            _recentProjects.Add(project);
            DisplayedProjects.Add(new OpenableProject(this, project));
        }
        LoadProject(project);
    }
    /// <summary>
    /// Final Step of the Loading Process.
    /// Important (for the old and new Project):
    /// The ViewModelPath, the ViewPath and all Views in the Project will be added by the SolutionExplorerViewModel,
    /// because he knows all the files and folders in the project.
    /// </summary>
    /// <param name="project">The Project which should be loaded</param>
    public void LoadProject(Project project)
    {
        EditorController controller = new EditorController();
        _mainViewModel.Content = controller.Init(project);
        ControlsCreator.EditorController = controller;
        // Saving the Editor Controller in the Top Navigation View to open views again.
        if (_mainViewModel.TopNavigationView.DataContext is TopNavigationViewModel viewModel)
            viewModel.EditorController = controller;
        SaveRecentProjectsToFile();
    }

    /// <summary>
    /// Opens a File Picker Dialog (https://github.com/AvaloniaUI/AvaloniaUI.QuickGuides/tree/main/FileOps)
    /// Only allows the user to choose .sln Files.
    /// </summary>
    /// <returns>The chosen File.</returns>
    /// <exception cref="NullReferenceException">Missing StorageProvider instance.</exception>
    private async Task<IStorageFile?> DoOpenFilePickerAsync()
    {
        
        // See IoCFileOps project for an example of how to accomplish this.
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");
        
        var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open Solution File",
            AllowMultiple = false,
            FileTypeFilter = new []
            {
                new FilePickerFileType("Solution File")
                {
                    Patterns = new []
                    {
                        "*.sln"
                    }
                }
            }
        });

        return files?.Count >= 1 ? files[0] : null;
    }
    
}