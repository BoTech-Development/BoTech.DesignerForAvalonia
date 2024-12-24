using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using BoTech.AvaloniaDesigner.Controller.Editor;
using BoTech.AvaloniaDesigner.Services.PropertiesView;
using BoTech.AvaloniaDesigner.Templates.Editor.PropertiesView;
using BoTech.AvaloniaDesigner.Views;
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

    private List<TabContent> TabContents { get; } = new List<TabContent>();
    
    private PreviewController _previewController;
    private Control _currentControl = new();
    private MainViewModel _mainViewModel;
    public PropertiesViewModel(PreviewController previewController, MainViewModel mainViewModel)
    {
        //Creating all Templates
        List<IViewTemplate> templates = new List<IViewTemplate>();
        templates.Add(new LayoutViewTemplate());
        TabContent layout = new TabContent("Layout", templates);
        layout.vm = this;
        TabContents.Add(layout);

        _mainViewModel = mainViewModel;
        this.UpdateView();
        
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
        
        UpdateView();
    }
    
    /// <summary>
    /// Applies the Prerendered Content of each TabContent Object to the View.
    /// </summary>
    private void UpdateView()
    {
        // Removing all Tabs
        Tabs.Items.Clear();
        // Converting
        foreach (TabContent content in TabContents)
        {
            TabItem tab = new TabItem();
            tab.Header = content.Name;
            tab.Content = new ScrollViewer()
                {
                    Content = content.PreRenderedContent,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    MaxHeight = 800
                    // TODO: Correct the Scroll Viewer
                  //  MaxHeight = _mainViewModel.Bounds.Height,
                  //  MaxWidth = _mainViewModel.Bounds.Width,
                };
            Tabs.Items.Add(tab);
        }
    }
    /// <summary>
    /// This Class represents the Content of a Tab. Each Tab has one Tab Content.
    /// The Tab content consist of a List of templates, which can be rednered by the <see cref="TabContent"><see cref="Render"/></see> Method.
    /// </summary>
    public class TabContent(string name, List<IViewTemplate> templates)
    {
        public string Name {get; set;} = name;
        public List<IViewTemplate> Templates { get; set; } = templates;

        public Control PreRenderedContent { get; set; } = new TextBlock()
        {
            Text = "Please wait...",
        };
        /// <summary>
        /// The refernce to the vm is needed to update the Tabs Property when for instance a repaint was called
        /// </summary>
        public PropertiesViewModel? vm { get; set; } 
        private List<Control> _preRenderedControls = new List<Control>();
        
        public void Render(Control control)
        {
            StackPanel stackPanel = new StackPanel();
            _preRenderedControls.Clear();
            foreach (IViewTemplate viewTemplate in Templates)
            {
                Expander expander = new Expander()
                {
                    Header = viewTemplate.Name,
                    Content = viewTemplate.GetViewTemplateForControl(control, this),
                };
                
                stackPanel.Children.Add(expander);
                // Saving the new Expander to the PreRenderedControls List.
                // It is needed to cache the Created Controls to make it easier to Render when only one ViewTemplate of the Collection of ViewTemplates has changed.
                _preRenderedControls.Add(expander);
            }
            PreRenderedContent = stackPanel;
        }
        /// <summary>
        /// This Method Handle the OnSelectedConstructorChanged Event from a ConstructorModel.
        /// This Method is needed because the ConstructorModel can not rerender itself because it can not access the view to set or add the new Controls in the PropertiesView.
        /// When calling the Method the selected ViewTemplate will be rerendered and all StandardViewTemplates.
        /// 
        /// </summary>
        /// <param name="viewTemplateName">The Name of the View Template where the Instance of the ControlsCreatorObject class is located in.</param>
        /// <param name="currentControl">The Control which was Selected by the User.</param>
    
        public void RenderOnlySelectedTemplate(string viewTemplateName, Control currentControl)
        {
            IViewTemplate? template = null;
            Control newControl = new();
            if ((template = Templates.Find(t => t.Name == viewTemplateName)) != null)
            {
                newControl = template.GetRerenderedViewTemplateForControl(currentControl, this);
            }
            // Find the Correct Index in the PreRenderedControls List
            int index = Templates.FindIndex(t => t.Name == viewTemplateName);
            _preRenderedControls[index] = newControl;
            // Update the PreRenderedControl var:
            StackPanel stackPanel = new StackPanel();
            foreach (Control control in _preRenderedControls)
            {
                stackPanel.Children.Add(control);
            }
            PreRenderedContent = stackPanel;
            
            // Apply the new Controls to the View
            
            vm?.UpdateView();
        }
    }
}