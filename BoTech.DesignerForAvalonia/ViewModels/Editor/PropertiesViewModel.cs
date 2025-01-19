using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using BoTech.DesignerForAvalonia.Services.PropertiesView;
using BoTech.DesignerForAvalonia.Views;
using BoTech.DesignerForAvalonia.Controller.Editor;
using BoTech.DesignerForAvalonia.Models.XML;
using BoTech.DesignerForAvalonia.Templates.Editor.PropertiesView;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.ViewModels.Editor;

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
    
    private EditorController _editorController;
    

    public PropertiesViewModel(EditorController editorController)
    {
        //Creating all Templates
        
        // Layout
        List<IViewTemplate> templates = new List<IViewTemplate>();
        templates.Add(new LayoutViewTemplate());
        TabContent layout = new TabContent("Layout", templates);
        layout.vm = this;
        TabContents.Add(layout);
        // Appearance
        templates = new List<IViewTemplate>();
        templates.Add(new AppearanceViewTemplate());
        TabContent appearance = new TabContent("Appearance", templates);
        appearance.vm = this;
        TabContents.Add(appearance);
        // Content
        templates = new List<IViewTemplate>();
        templates.Add(new ContentViewTemplate());
        TabContent content = new TabContent("Content", templates);
        content.vm = this;
        TabContents.Add(content);
        // Input
        templates = new List<IViewTemplate>();
        templates.Add(new InputViewTemplate());
        TabContent input = new TabContent("Input", templates);
        input.vm = this;
        TabContents.Add(input);

        
        
        _editorController = editorController;
        _editorController.PropertiesViewModel = this;
    }
    /// <summary>
    /// This Method is called by the Code-Behind.
    /// The Code Behind has to set some Properties like the CornerBorder.
    ///  Only when these properties are set the propertiesView can render.
    /// </summary>
    public void Init()
    {
        UpdateView();
    }
    /// <summary>
    /// Render the IViewTemplates with the correct data from the Control injected. 
    /// </summary>
    /// <param name="control"></param>
    public void RenderForControl(XmlControl xmlControl)
    {
        // Prerendering
        foreach (TabContent content in TabContents)
        {
            content.Render(xmlControl);
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
                    //VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                    //HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
                    //Height = 800,
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
        
        public void Render(XmlControl xmlControl)
        {
            StackPanel stackPanel = new StackPanel();
            _preRenderedControls.Clear();
            foreach (IViewTemplate viewTemplate in Templates)
            {
                Expander expander = new Expander()
                {
                    Header = viewTemplate.Name,
                    Content = viewTemplate.GetViewTemplateForControl(xmlControl, this),
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
    
        public void RenderOnlySelectedTemplate(string viewTemplateName, XmlControl currentXmlControl)
        {
            IViewTemplate? template = null;
            Control newControl = new();
            if ((template = Templates.Find(t => t.Name == viewTemplateName)) != null)
            {
                newControl = template.GetRerenderedViewTemplateForControl(currentXmlControl, this);
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