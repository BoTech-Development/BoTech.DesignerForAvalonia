using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using BoTech.AvaloniaDesigner.Controller.Editor;

namespace BoTech.AvaloniaDesigner.Services.PropertiesView;

/// <summary>
/// This class is a helper Class to build up the Properties View.
/// It creates Standardized Editatable Boxes for the Properties of an Control.
/// </summary>

public static class ControlsCreator
{
    public static PreviewController? PreviewController { get; set; } = new();
    public static readonly List<string> SupportedPrimitiveTypes = new List<string> { "Boolean", "bool", "Int16","UInt16","Int32","UInt32","Int64", "UInt64", "Single", "Double", "Decimal", "String", "string" };
    /// <summary>
    /// A List of all Types which do not need an editable Constructor and are stored under the namespace Avalonia.
    /// In this case it can be Enums like HorizontalAlignment.
    /// </summary>
    public static readonly List<string> SupportedAvaloniaTypes = new List<string> { "HorizontalAlignment", "VerticalAlignment", "FontWeight", "FontStyle" };
    /// <summary>
    /// This Method creates a Control to edit a Property for a specific Control
    /// </summary>
    /// <param name="control">The referenced Control</param>
    /// <param name="property">The Name of the Property of the Control</param>
    /// <param name="options">View Options</param>
    /// <returns>One Avalonia Control when the Property was found and can display (Type casting). If it is not the case a TextBlock with the Error Message will be returned.  </returns>
    public static Control CreateEditBox(Control control, string property, EditBoxOptions options)
    {
        Type controlType = control.GetType();
       // if (controlType.BaseType != typeof(Control))
        //{
        PropertyInfo[] propertyInfos = controlType.GetProperties();
        PropertyInfo? propertyInfo;
        if ((propertyInfo = propertyInfos.Where(p => p.Name == property).FirstOrDefault()) != null)
        {
            return ControlsCreator.CreateEditBox(propertyInfo, control, options);
        }
        
        return new TextBlock()
        {
            Text = "Can not load a Template for: " + property,
        };
    }

  
    public static Control CreateEditBox(PropertyInfo propertyInfo, Control control, EditBoxOptions options)
    {
        if (options == EditBoxOptions.Auto)
        {
            switch (propertyInfo.PropertyType.Name)
            {
                // All possible bool based Types:
                case "Boolean": return CreateEditBoxForBoolean(propertyInfo, control);
                case "bool": return CreateEditBoxForBoolean(propertyInfo, control);
                        
                // All possible Integer based Types. 
                        
                case "Int16": return CreateEditBoxForInteger(propertyInfo, control);
                case "UInt16": return CreateEditBoxForInteger(propertyInfo, control);
                case "Int32": return CreateEditBoxForInteger(propertyInfo, control);
                case "UInt32": return CreateEditBoxForInteger(propertyInfo, control);
                case "Int64": return CreateEditBoxForInteger(propertyInfo, control);
                case "UInt64": return CreateEditBoxForInteger(propertyInfo, control);
                        
                // All Floating-Point primitive Types:
                        
                case "Single":
                    if ((float)propertyInfo.GetValue(control) != float.PositiveInfinity && (float)propertyInfo.GetValue(control) != float.NegativeInfinity)
                    {
                        return CreateEditBoxForFloatingPoint(propertyInfo, control);
                    }
                    return new TextBlock()
                    {
                        Text = "Can not display: " + propertyInfo.Name + "because its +|- infinite.",
                    };
                case "Double":
                    if ((double)propertyInfo.GetValue(control) != double.PositiveInfinity && (double)propertyInfo.GetValue(control) != double.NegativeInfinity)
                    {
                        return CreateEditBoxForFloatingPoint(propertyInfo, control);
                    }
                    return new TextBlock()
                    {
                        Text = "Can not display: " + propertyInfo.Name + "because its +|- infinite.",
                    };
                case "Decimal": return CreateEditBoxForFloatingPoint(propertyInfo, control);
                            
                            
                // Primitive Type string and Class String are handled the same way:
                case "string": return CreateEditBoxForString(propertyInfo, control);
                case "String": return CreateEditBoxForString(propertyInfo, control);
                    
                // Avalonia Types:
                case "Thickness": return ControlsCreatorAvalonia.CreateEditBoxForThickness(propertyInfo, control);
                case "HorizontalAlignment": return ControlsCreatorAvalonia.CreateEditableControlForEnum(propertyInfo, control);
                case "VerticalAlignment": return ControlsCreatorAvalonia.CreateEditableControlForEnum(propertyInfo, control);
                case "FontWeight": return ControlsCreatorAvalonia.CreateEditableControlForEnum(propertyInfo, control);
                case "FontStyle": return ControlsCreatorAvalonia.CreateEditableControlForEnum(propertyInfo, control);
                
                
              

            }
   
            
        }
        else if(options == EditBoxOptions.EmbedBindingsView)
        {
                    
        }
        return new TextBlock()
        {
            Text = "Can not load a Template for: " + propertyInfo.Name,
        };
    }
    /// <summary>
    /// This Method is a Helper Method to add the Property name to the Control. 
    /// </summary>
    /// <param name="control"></param>
    /// <param name="propertyInfo"></param>
    /// <returns></returns>
    public static StackPanel AddEditBoxToStackPanel(Control control, PropertyInfo propertyInfo)
    {
        StackPanel stackPanel = new StackPanel();
        stackPanel.Orientation = Orientation.Horizontal;
        stackPanel.Children.Add(new TextBlock()
        {
            Text = propertyInfo.Name + ":",
            VerticalAlignment = VerticalAlignment.Center,
        });
        stackPanel.Children.Add(control);
        return stackPanel;
    }
    
    private static Control CreateEditBoxForBoolean(PropertyInfo propertyInfo, Control control)
    {
        CheckBox cb = new CheckBox()
        {
            Content = propertyInfo.Name,
            IsChecked = (bool)propertyInfo.GetValue(control, null)!,
            IsEnabled = propertyInfo.CanWrite
        };
       
        cb.IsCheckedChanged += (s, e) =>
        {
            if (PreviewController != null)
            {
                PreviewController.OnPropertyInPropertiesViewChanged(control, propertyInfo, cb.IsChecked);
            }
        };
        return cb;
    }
    /// <summary>
    /// External Method which is an extraction for all Integer Based Types
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    private static Control CreateEditBoxForInteger(PropertyInfo propertyInfo, Control control)
    {
        NumericUpDown numericUpDown = new NumericUpDown()
        {
            Text = propertyInfo.Name,
            FormatString = "0",
            Minimum = 0,
            Value = Convert.ToDecimal(propertyInfo.GetValue(control, null)),
            Increment = 1,
        };
        numericUpDown.ValueChanged += (sender, args) =>
        {
            if (PreviewController != null)
            {
                PreviewController.OnPropertyInPropertiesViewChanged(control, propertyInfo, numericUpDown.Value);
            }
        };
        
        return AddEditBoxToStackPanel(numericUpDown, propertyInfo);
    }
    /// <summary>
    /// External Method which is an extraction for all Integer Based Types
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    private static Control CreateEditBoxForFloatingPoint(PropertyInfo propertyInfo, Control control)
    {
        NumericUpDown numericUpDown = new NumericUpDown()
        {
            Text = propertyInfo.Name,
            FormatString = "0.00",
            Minimum = 0,
            Value = Convert.ToDecimal(propertyInfo.GetValue(control, null))!,
            Increment = 1,
        };
        numericUpDown.ValueChanged += (s, e) =>
        {
            if (PreviewController != null)
            {
                PreviewController.OnPropertyInPropertiesViewChanged(control, propertyInfo, numericUpDown.Value);
            }
        };
        return AddEditBoxToStackPanel(numericUpDown, propertyInfo);
    }
    /// <summary>
    /// External Method which is an extraction for all Integer Based Types
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    private static Control CreateEditBoxForString(PropertyInfo propertyInfo, Control control)
    {
        TextBox tb = new TextBox()
        {
            Text = (string)propertyInfo.GetValue(control, null)!,
            IsEnabled = propertyInfo.CanWrite,
        };
        tb.TextChanged += (s, e) =>
        {
            if (PreviewController != null)
            {
                PreviewController.OnPropertyInPropertiesViewChanged(control, propertyInfo, tb.Text);
            }
        };
        // PreviewController.OnPropertyInPropertiesViewChanged(control, propertyInfo.Name, tb.Text);
        return AddEditBoxToStackPanel(tb, propertyInfo);
    }
}