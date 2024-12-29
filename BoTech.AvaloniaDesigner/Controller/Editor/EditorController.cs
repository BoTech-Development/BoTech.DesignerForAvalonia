using System;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Layout;
using BoTech.AvaloniaDesigner.Models.Editor;
using BoTech.AvaloniaDesigner.ViewModels.Editor;
using BoTech.AvaloniaDesigner.Views.Editor;

namespace BoTech.AvaloniaDesigner.Controller.Editor;

public class EditorController
{
 
    // All Views that are needed for the Editor:
    public SolutionExplorerView SolutionExplorerView { get; set; }
    public PreviewView PreviewView { get; set; }
    public ItemsExplorerView ItemsView { get; set; } 
    public ViewHierarchyView ViewHierarchyView { get; set; }
    public PropertiesView PropertiesView { get; set; }
    
    
    public ViewHierarchyViewModel? ViewHierarchyViewModel { get; set; }
    public PropertiesViewModel? PropertiesViewModel { get; set; }
    
    /// <summary>
    /// What the user want to do
    /// </summary>
    public EDragAndDropOperation Operation { get; set; }
    /// <summary>
    /// Selected Control which can be dropped to the Preview.
    /// </summary>
    public Control? CurrentControl { get; set; }
    /// <summary>
    /// The Selected Control which the User can Select from the Hierarchy View
    /// </summary>
    public Control? SelectedControl { get; set; }


    /// <summary>
    /// Will be set by the <see cref="PreviewViewModel"/> when the user hase changed something.
    /// </summary>
    public Grid PreviewContent { get; set; }
   
    
    public EditorController()
    {
        Operation = EDragAndDropOperation.None;
        CurrentControl = null;
        
        // Init PreviewContent
        TextBlock pleaseWait = new TextBlock()
        {
            Text = "Please wait while initialisation...",
        };
        PreviewContent = new Grid();
        PreviewContent.Children.Add(pleaseWait);
        
    }
    /// <summary>
    /// The Controller has to be Initialized before it can run.
    /// The Init Method is used to create all Views.
    /// <returns>A grid is returned that contains all views belonging to the editor.</returns>
    /// </summary>
    public StackPanel Init(string selectedPath, string projectName)
    {
        SolutionExplorerView = new SolutionExplorerView()
        {
            DataContext = new SolutionExplorerViewModel(projectName, selectedPath, this),
            //[Grid.ColumnProperty] = 0
        };
        PreviewView = new PreviewView()
        {
            DataContext = new PreviewViewModel(this)
            
        };
        ItemsView = new ItemsExplorerView()
        {
            DataContext = new ItemsExplorerViewModel(this)
        };
        ViewHierarchyView = new ViewHierarchyView()
        {
            DataContext = new ViewHierarchyViewModel(this)
        };
        PropertiesView = new PropertiesView()
        {
            DataContext = new PropertiesViewModel(this)
        };
        return new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            Children =
            {
                SolutionExplorerView,
                ItemsView,
                PreviewView,
                ViewHierarchyView,
                PropertiesView,
            },
           // ColumnDefinitions = new ColumnDefinitions("Auto, Auto, Auto, Auto ,Auto"),
        };   
    }

    
    /// <summary>
    /// This Method tries to apply the new Value to the referenced Property of the Control
    /// </summary>
    /// <param name="control"></param>
    /// <param name="propertyInfo"></param>
    /// <param name="newValue"></param>
    public void OnPropertyInPropertiesViewChanged(Control control, PropertyInfo propertyInfo, object? newValue)
    {
        try
        {
            propertyInfo.SetValue(control, newValue);
        }
        catch (Exception e)
        {
            // When the Property could not be converted
            if (newValue != null)
            {
                if (newValue.GetType() != propertyInfo.PropertyType)
                {
                    try
                    {
                        // Typecasting when the Type of the new Value is not equals with the Requested Type of the Property
                        propertyInfo.SetValue(control, Convert.ChangeType(newValue, propertyInfo.PropertyType));
                    }
                    catch (Exception e2)
                    {
                        Console.WriteLine($"Exception in OnPropertyInPropertiesViewChanged: {e2}");
                    }
                }
                else
                {
                    // Property might be readonly.
                    Console.WriteLine(e);
                }
            }
            else
            {
                // Property might be readonly.
                Console.WriteLine(e);
            }
        }
        
    }
    /// <summary>
    /// Will be executed when the PreviewContent was changed by the PreviewViewModel.
    /// <b>Important this Method must be called by each class which change the Preview Content.</b>
    /// This Event is needed, because other Views like the ViewHierarchy View has to reload the Tree View.
    /// </summary>
    public void OnPreviewContentChanged()
    {
        if (ViewHierarchyViewModel != null) ViewHierarchyViewModel.Reload();
    }
    /// <summary>
    /// Event which will be called by the ViewHierarchyViewModel 
    /// </summary>
    public void OnSelectedControlChanged()
    {
        if (PropertiesViewModel != null && SelectedControl != null) PropertiesViewModel.RenderForControl(SelectedControl);
    }

    public void StartDrag(Control? control)
    {
        CurrentControl = control;
        Operation = EDragAndDropOperation.DropObjectToPreview;
    }

    public void DraggingPaused()
    {
        Operation = EDragAndDropOperation.Paused;
    }
    public void EndDrag()
    {
        CurrentControl = null;
        Operation = EDragAndDropOperation.None;
    }
}