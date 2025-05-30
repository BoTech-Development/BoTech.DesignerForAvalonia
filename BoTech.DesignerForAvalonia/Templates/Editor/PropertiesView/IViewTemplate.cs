
using System.Collections.Generic;
using Avalonia.Controls;
using BoTech.DesignerForAvalonia.Models.XML;
using BoTech.DesignerForAvalonia.ViewModels.Editor;

namespace BoTech.DesignerForAvalonia.Templates.Editor.PropertiesView;

public interface IViewTemplate
{
     /// <summary>
     /// Will be displayed on the Expander Header.
     /// The Name has to be unique.
     /// </summary>
     public string Name { get; }
     /// <summary>
     /// In this each view Template has to store all its StandardViewTemplates.
     /// This is necessary to Handle the Rerender event of an ControlsCreatorObject Class.
     /// The PropertiesViewModel will select the correct ViewTemplate which has called the event and execute the Rerender Method (GetViewTemplateForControl()).
     /// </summary>
     public List<StandardViewTemplate> StandardViewTemplates { get; set;  }

     /// <summary>
     /// This Method Creates all static Editable Controls which are needed to edit the Propertiesd of the current Control.
     /// For Instance, it creates the Margin and Padding Box in the LayoutViewTemplate
     /// </summary>
     /// <param name="xmlControl"></param>
     /// <param name="tabContent"></param>
     /// <returns></returns>
     public Control GetViewTemplateForControl(XmlControl xmlControl, PropertiesViewModel.TabContent tabContent);

     /// <summary>
     /// When the ViewTemplate uses StandardViewTemplates, a repaint (Rerender) might be requested by the ControlsCreatorObject.
     /// This Method processes these Repaints.
     /// </summary>
     /// <param name="xmlControl">The Selected Control</param>
     /// <param name="tabContent">The Current TabContent Model where this View Template is located in.</param>
     /// <returns>The Editable Controls</returns>
     public Control GetRerenderedViewTemplateForControl(XmlControl xmlControl,
          PropertiesViewModel.TabContent tabContent)
     {
          StackPanel stackPanel = new StackPanel();
          foreach (StandardViewTemplate standardViewTemplate in StandardViewTemplates)
          {
               stackPanel.Children.Add(standardViewTemplate.GetViewTemplateForControl(xmlControl, tabContent, Name));
          }
          return stackPanel;
     }

}