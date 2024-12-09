using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using BoTech.AvaloniaDesigner.Controller.Editor;
using BoTech.AvaloniaDesigner.Templates.Editor.PropertiesView;
using ReactiveUI;

namespace BoTech.AvaloniaDesigner.ViewModels.Editor;

public class PropertiesViewModel : ViewModelBase
{
    private TabControl _tabs = new TabControl();
    /// <summary>
    /// Tabs for the View
    /// </summary>
    public TabControl Tabs
    {
        get => _tabs; 
        set => this.RaiseAndSetIfChanged(ref _tabs, value); 
    }
    public List<TabContent> TabContents { get; } = new List<TabContent>();
    
    private PreviewController _previewController;
    private Control _currentControl = new();
    
    public PropertiesViewModel(PreviewController previewController)
    {
        //Creating all Templates
        List<IViewTemplate> templates = new List<IViewTemplate>();
        templates.Add(new LayoutViewTemplate());
        TabContent layout = new TabContent("Layout", templates);
        TabContents.Add(layout);

        
        this.Render();
        
        _previewController = previewController;
        _previewController.PropertiesViewModel = this;
    }
    /// <summary>
    /// Render the IViewTemplates with the correct data from the Control injected. 
    /// </summary>
    /// <param name="control"></param>
    public void RenderForControl(Control control)
    {
        _currentControl = control;
        
        // Prerendering
        foreach (TabContent content in TabContents)
        {
            content.Render(control);
        }
        
        Render();
    }
    /// <summary>
    /// Render the IViewTemplates in connection with the current Control
    /// </summary>
    private void Render()
    {
        // Removing all Tabs
        Tabs.Items.Clear();
        // Converting
        foreach (TabContent content in TabContents)
        {
            TabItem tab = new TabItem();
            tab.Header = content.Name;
            tab.Content = content.PreRenderedContent;
            Tabs.Items.Add(tab);
        }
    }
    
    public class TabContent(string name, List<IViewTemplate> templates)
    {
        public string Name {get; set;} = name;
        public List<IViewTemplate> Templates { get; set; } = templates;

        public Control PreRenderedContent { get; set; } = new TextBlock()
        {
            Text = "Please wait...",
        };

        public void Render(Control control)
        {
            StackPanel stackPanel = new StackPanel();
            
            foreach (IViewTemplate viewTemplate in Templates)
            {
                Expander expander = new Expander()
                {
                    Header = viewTemplate.Name,
                    Content = viewTemplate.GetViewTemplateForControl(control),
                };
                
                stackPanel.Children.Add(expander);
            }
            PreRenderedContent = stackPanel;
        }
    }
}