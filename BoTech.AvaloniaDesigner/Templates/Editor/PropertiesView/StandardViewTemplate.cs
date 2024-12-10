using System.Collections.Generic;
using Avalonia.Controls;
using BoTech.AvaloniaDesigner.Services.PropertiesView;

namespace BoTech.AvaloniaDesigner.Templates.Editor.PropertiesView;

/// <summary>
/// A wrapper Class for the Controls Creator. This class can be used when 
/// </summary>

public class StandardViewTemplate : IViewTemplate
{
    public string Name { get; } = "Standard View";
    /// <summary>
    /// List of all Properties which are located under the Expander.
    /// </summary>
    public List<ReferencedProperty> ReferencedProperties { get; set; } = new List<ReferencedProperty>();
    public Control GetViewTemplateForControl(Control control)
    {
        StackPanel stackPanel = new StackPanel();
        foreach (ReferencedProperty referencedProperty in ReferencedProperties)
        {
            stackPanel.Children.Add(ControlsCreator.CreateEditBox(control, referencedProperty.PropertyName, referencedProperty.Options));
        }
        return stackPanel;
    }
    /// <summary>
    /// The class ReferencedProperty is a Model for each Property which is nested under the Expander.
    /// </summary>
    /// <param name="propertyName">The PropertyName is the Name of the Property which is stored in the referenced Control.</param>
    /// <param name="options">Display Optrions</param>
    public class ReferencedProperty(string propertyName, EditBoxOptions options)
    {
        public string PropertyName { get; set; } = propertyName;
        public EditBoxOptions Options { get; set; } = options;
    }
}