using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using BoTech.AvaloniaDesigner.Models.XML;
using BoTech.AvaloniaDesigner.Services.PropertiesView;
using BoTech.AvaloniaDesigner.ViewModels.Editor;

namespace BoTech.AvaloniaDesigner.Templates.Editor.PropertiesView;

public class AppearanceViewTemplate : IViewTemplate
{
    public string Name { get; } = "Appearance";
    public List<StandardViewTemplate> StandardViewTemplates { get; set; }
    public Control GetViewTemplateForControl(XmlControl xmlControl, PropertiesViewModel.TabContent tabContent)
    {
        // Main StackPanel
        StackPanel stackPanel = new StackPanel();
		
        StandardViewTemplates = new List<StandardViewTemplate>();
        StandardViewTemplate stdViewTemplate = new StandardViewTemplate()
        {
            ReferencedProperties =
            {
                new StandardViewTemplate.ReferencedProperty("IsEnabled", xmlControl.Control, EditBoxOptions.Auto),
                new StandardViewTemplate.ReferencedProperty("IsVisible", xmlControl.Control, EditBoxOptions.Auto),
                new StandardViewTemplate.ReferencedProperty("Classes", xmlControl.Control, EditBoxOptions.Auto),
                new StandardViewTemplate.ReferencedProperty("Styles", xmlControl.Control, EditBoxOptions.Auto), 
                new StandardViewTemplate.ReferencedProperty("FontFamily", xmlControl.Control, EditBoxOptions.Auto),
                new StandardViewTemplate.ReferencedProperty("FontSize", xmlControl.Control, EditBoxOptions.Auto),
                new StandardViewTemplate.ReferencedProperty("FontStyle", xmlControl.Control, EditBoxOptions.Auto),
                new StandardViewTemplate.ReferencedProperty("FontWeight", xmlControl.Control, EditBoxOptions.Auto),
                new StandardViewTemplate.ReferencedProperty("Foreground", xmlControl.Control, EditBoxOptions.Auto),
                new StandardViewTemplate.ReferencedProperty("Background", xmlControl.Control, EditBoxOptions.Auto),
                new StandardViewTemplate.ReferencedProperty("BorderBrush", xmlControl.Control, EditBoxOptions.Auto),
            }
        };
        
        
        
        // Saving the Standard View Template for the event handling
        StandardViewTemplates.Add(stdViewTemplate);
        stackPanel.Children.Add(stdViewTemplate.GetViewTemplateForControl(xmlControl, tabContent, Name));
        return stackPanel;
    }
}