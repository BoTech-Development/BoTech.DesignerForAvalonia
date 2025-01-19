using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using BoTech.DesignerForAvalonia.Models.XML;
using BoTech.DesignerForAvalonia.Services.PropertiesView;
using BoTech.DesignerForAvalonia.ViewModels.Editor;

namespace BoTech.DesignerForAvalonia.Templates.Editor.PropertiesView;

/// <summary>
/// A wrapper Class for the Controls Creator. This class can be used when you want to create an editable Box for one of the Avalonia Properties which are defined in the ControlsCreator Class.
/// </summary>

public class StandardViewTemplate 
{
    public string Name { get; } = "Standard View";
    /// <summary>
    /// List of all Properties which are located under the Expander.
    /// </summary>
    public List<ReferencedProperty> ReferencedProperties { get; set; } = new List<ReferencedProperty>();
    // Is not necessary in this Class. 
    public List<StandardViewTemplate> StandardViewTemplates { get; set; } = new List<StandardViewTemplate>();
    
    private bool _firstRender = true;
    // Note this Method rerenders the Referenced Property when this Method called more than one time.
    public Control GetViewTemplateForControl(XmlControl xmlControl, PropertiesViewModel.TabContent tabContent, string viewTemplateName)
    {
        if (_firstRender)
        {
            // If it is necessary new Instances of the ControlsCreatorObject Class will be created and stored.
            StackPanel stackPanel = new StackPanel();
            foreach (ReferencedProperty referencedProperty in ReferencedProperties)
            {
                if (referencedProperty.PropertyInfo == null)
                {
                    // The Property is not available in the selected Control:
                    stackPanel.Children.Add(new TextBlock()
                    {
                        Text = "The Property " + referencedProperty.PropertyName + " does not exist in the selected Control.",
                        Margin = new Thickness(0,5,0,0),
                        Foreground = Brushes.Orange,
                    });
                }
                else
                {
                    if (ControlsCreator.SupportedPrimitiveTypes.Contains(referencedProperty.PropertyInfo.PropertyType
                            .Name) ||
                        ControlsCreator.SupportedAvaloniaTypes.Contains(referencedProperty.PropertyInfo.PropertyType
                            .Name))
                    {
                        stackPanel.Children.Add(ControlsCreator.CreateEditBox(xmlControl, referencedProperty.PropertyName,
                            referencedProperty.Options));
                    }
                    else
                    {
                        // Create a new Editable Control with the ControlsCreatorObject class:
                        referencedProperty.ControlCreator = new ControlsCreatorObject(referencedProperty.PropertyInfo,
                            xmlControl, tabContent, viewTemplateName);
                        stackPanel.Children.Add(referencedProperty.ControlCreator.EditableControls);
                    }
                }
            }
            _firstRender = false;
            return stackPanel;
        }
        else
        {
            // If one of the ControlsCreatorObjects call a rerender event. This is the case when the user changed a Value or the Selected Constructor.
            StackPanel stackPanel = new StackPanel();
            foreach (ReferencedProperty referencedProperty in ReferencedProperties)
            {
                if (referencedProperty.PropertyInfo == null)
                {
                    // The Property is not available in the selected Control:
                    stackPanel.Children.Add(new TextBlock()
                    {
                        Text = "The Property " + referencedProperty.PropertyName + " does not exist in the selected Control.",
                        Margin = new Thickness(0,5,0,0),
                        Foreground = Brushes.Orange,
                    });
                }
                else
                {
                    if (ControlsCreator.SupportedPrimitiveTypes.Contains(referencedProperty.PropertyInfo.PropertyType.Name) ||
                        ControlsCreator.SupportedAvaloniaTypes.Contains(referencedProperty.PropertyInfo.PropertyType.Name))
                    {
                        stackPanel.Children.Add(ControlsCreator.CreateEditBox(xmlControl, referencedProperty.PropertyName, referencedProperty.Options));
                    }
                    else
                    {
                        // One of the Created instances have called a Rerender event.
                        if (referencedProperty.ControlCreator != null)
                        {
                            // Check if Something has changed.
                            //if (referencedProperty.ControlCreator.Rerender())
                            //{
                            referencedProperty.ControlCreator.Rerender();
                                stackPanel.Children.Add(referencedProperty.ControlCreator.EditableControls);
                            //}
                        }
                    }
                }
            }
            return stackPanel;
        }
    }
    /// <summary>
    /// The class ReferencedProperty is a Model for each Property which is nested under the Expander.
    /// </summary>
    /// <param name="propertyName">The PropertyName is the Name of the Property which is stored in the referenced Control.</param>
    /// <param name="options">Display Options</param>
    public class ReferencedProperty
    {
        public string PropertyName { get; set; } 
        public EditBoxOptions Options { get; set; } 
        /// <summary>
        /// The Property Info stores the Information about the property in the selected Control.
        /// It is <b>null</b> when the Selected Control hasn't this Property.
        /// For Instance a Button does not have a Text Property. Therefore, the PropertyInfo will be null.
        /// </summary>
        public PropertyInfo? PropertyInfo { get; set; }
        /// <summary>
        /// Will store an Instance of the ControlsCreatorClass when the given Property could not be handled by the ControlsCreator or the ControlsCreatorAvalonia Class.<br/>
        /// Can be null when it is not necessary when it works to create an editable Control with one of the named Classes. 
        /// </summary>
        public ControlsCreatorObject? ControlCreator { get; set; }

        public ReferencedProperty(string propertyName, Control control, EditBoxOptions options)
        {
            PropertyName = propertyName;
            Options = options;
            Type controlType = control.GetType();
            // if (controlType.BaseType != typeof(Control))
            //{
            PropertyInfo[] propertyInfos = controlType.GetProperties();
            PropertyInfo? propertyInfo;
            if ((propertyInfo = propertyInfos.Where(p => p.Name == propertyName).FirstOrDefault()) != null)
            {
               PropertyInfo = propertyInfo;
            }
        }
    }
}