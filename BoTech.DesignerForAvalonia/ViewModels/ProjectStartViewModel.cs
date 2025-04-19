using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using BoTech.DesignerForAvalonia.Controller.Editor;
using BoTech.DesignerForAvalonia.Models.Project;


using ReactiveUI;

namespace BoTech.DesignerForAvalonia.ViewModels;

public class ProjectStartViewModel : ViewModelBase
{
    private ObservableCollection<OpenableProject> _displayedProjects = new ObservableCollection<OpenableProject>();

    public ObservableCollection<OpenableProject> DisplayedProjects
    {
        get => _displayedProjects;
        set => this.RaiseAndSetIfChanged(ref _displayedProjects, value);
    }
    /// <summary>
    /// Opens a File Picker
    /// </summary>
    public ReactiveCommand<Unit, Unit> OpenProjectCommand { get; set; }
    /// <summary>
    /// Loads the Project and opens the editor View.
    /// </summary>
    public ReactiveCommand<Unit, Unit> LoadProjectCommand { get; set; }
    private ProjectController _projectController;
    
   
    
    private bool _openFilePickerSuccess = false;
    private IStorageFile? currentSolutionFile = null;


    public ProjectStartViewModel(MainViewModel mainViewModel)
    {
        _projectController = new ProjectController(mainViewModel);
        _projectController.Initialize();

        LoadProjectCommand = ReactiveCommand.Create(() =>
        {
            if (_openFilePickerSuccess && currentSolutionFile != null)
                _projectController.LoadProject(currentSolutionFile.Path.LocalPath,
                    currentSolutionFile.Path.AbsolutePath, currentSolutionFile.Name);
        });
        OpenProjectCommand = ReactiveCommand.CreateRunInBackground(OpenProject);

        // Save the Loaded List of recent Porjects in the DisplayedProjects Collection, so that they appear on the Screen
        if (_projectController.RecentProjects.Count > 0)
        {
            foreach (Project recentProject in _projectController.RecentProjects)
            {
                DisplayedProjects.Add(new OpenableProject(this, recentProject));
            }
        }
        else
        {
            DisplayedProjects.Add(new OpenableProject(this, new Project(){Name = "Welcome to the BoTech.DesignerForAvalonia.", SolutionFilePath = " Please select a project."}, true));
        }
    }

    /// <summary>
    /// Loads a Project from the Recent Project List.
    /// </summary>
    /// <param name="openableProject"></param>
    public void LoadOpenableProject(OpenableProject openableProject) => _projectController.LoadProject((Project)openableProject);
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