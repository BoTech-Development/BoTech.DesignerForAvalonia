using System.Collections.Generic;
using Avalonia.Controls;
using BoTech.DesignerForAvalonia.Models.XML;
using BoTech.DesignerForAvalonia.Services.PropertiesView;
using BoTech.DesignerForAvalonia.ViewModels.Editor;

namespace BoTech.DesignerForAvalonia.Templates.Editor.PropertiesView;

public class ContentViewTemplate : IViewTemplate 
{
    public string Name { get; } = "Content";
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
                new StandardViewTemplate.ReferencedProperty("Text", xmlControl.Control, EditBoxOptions.Auto)
            }
        };
        
        stackPanel.Children.Add(new TextBlock()
        {
            Text = "Note: All Content Properties will be available in Version V1.2.x of this Product. \n For more details visit our Roadmap: https://aka.botech.dev/go/DesignerForAvalonia/Roadmap"
        });
        
        
        
        // Saving the Standard View Template for the event handling
        StandardViewTemplates.Add(stdViewTemplate);
        stackPanel.Children.Add(stdViewTemplate.GetViewTemplateForControl(xmlControl, tabContent, Name));
        return stackPanel;
    }
}