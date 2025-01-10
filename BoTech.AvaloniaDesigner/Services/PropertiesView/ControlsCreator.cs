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
using BoTech.AvaloniaDesigner.Models.XML;

namespace BoTech.AvaloniaDesigner.Services.PropertiesView;

/// <summary>
/// This class is a helper Class to build up the Properties View.
/// It creates Standardized Editatable Boxes for the Properties of an Control.
/// </summary>

public static class ControlsCreator
{
    public static EditorController? EditorController { get; set; } = new();
    public static readonly List<string> SupportedPrimitiveTypes = new List<string> { "Boolean", "bool", "Int16","UInt16","Int32","UInt32","Int64", "UInt64", "Single", "Double", "Decimal", "String", "string" };
    /// <summary>
    /// A List of all Types which do not need an editable Constructor and are stored under the namespace Avalonia.
    /// In this case it can be Enums like HorizontalAlignment.
    /// </summary>
    public static readonly List<string> SupportedAvaloniaTypes = new List<string> { "HorizontalAlignment", "VerticalAlignment", "FontWeight", "FontStyle", "IBrush", "FontFamily" };
    /// <summary>
    /// This Method creates a Control to edit a Property for a specific Control
    /// </summary>
    /// <param name="xmlControl">The referenced Control</param>
    /// <param name="property">The Name of the Property of the Control</param>
    /// <param name="options">View Options</param>
    /// <returns>One Avalonia Control when the Property was found and can display (Type casting). If it is not the case a TextBlock with the Error Message will be returned.  </returns>
    public static Control CreateEditBox(XmlControl xmlControl, string property, EditBoxOptions options)
    {
        Type controlType = xmlControl.Control.GetType();
       // if (controlType.BaseType != typeof(Control))
        //{
        PropertyInfo[] propertyInfos = controlType.GetProperties();
        PropertyInfo? propertyInfo;
        if ((propertyInfo = propertyInfos.Where(p => p.Name == property).FirstOrDefault()) != null)
        {
            return ControlsCreator.CreateEditBox(xmlControl, propertyInfo,  options);
        }
        
        return new TextBlock()
        {
            Text = "Can not load a Template for: " + property,
            Margin = new Thickness(0,5,0,0),
            Foreground = Brushes.Orange
        };
    }

  
    public static Control CreateEditBox(XmlControl xmlControl, PropertyInfo propertyInfo,  EditBoxOptions options)
    {
        if (options == EditBoxOptions.Auto)
        {
            switch (propertyInfo.PropertyType.Name)
            {
                // All possible bool based Types:
                case "Boolean": return CreateEditBoxForBoolean(propertyInfo, xmlControl);
                case "bool": return CreateEditBoxForBoolean(propertyInfo, xmlControl);
                        
                // All possible Integer based Types. 
                        
                case "Int16": return CreateEditBoxForInteger(propertyInfo, xmlControl);
                case "UInt16": return CreateEditBoxForInteger(propertyInfo, xmlControl);
                case "Int32": return CreateEditBoxForInteger(propertyInfo, xmlControl);
                case "UInt32": return CreateEditBoxForInteger(propertyInfo, xmlControl);
                case "Int64": return CreateEditBoxForInteger(propertyInfo, xmlControl);
                case "UInt64": return CreateEditBoxForInteger(propertyInfo, xmlControl);
                        
                // All Floating-Point primitive Types:
                        
                case "Single":
                    if ((float)propertyInfo.GetValue(xmlControl.Control) != float.PositiveInfinity && (float)propertyInfo.GetValue(xmlControl.Control) != float.NegativeInfinity)
                    {
                        return CreateEditBoxForFloatingPoint(propertyInfo, xmlControl);
                    }
                    return new TextBlock()
                    {
                        Text = "Can not display: " + propertyInfo.Name + " because its +|- infinite.",
                    };
                case "Double":
                    double value = (double)propertyInfo.GetValue(xmlControl.Control);
                    if (value != double.PositiveInfinity && value != double.NegativeInfinity)
                    {
                        return CreateEditBoxForFloatingPoint(propertyInfo, xmlControl);
                    }
                    return new TextBlock()
                    {
                        Text = "Can not display: " + propertyInfo.Name + " because its +|- infinite.",
                    };
                case "Decimal":  return CreateEditBoxForFloatingPoint(propertyInfo, xmlControl);
                            
                            
                // Primitive Type string and Class String are handled the same way:
                case "string": return CreateEditBoxForString(propertyInfo, xmlControl);
                case "String": return CreateEditBoxForString(propertyInfo, xmlControl);
                    
                // Avalonia Types:
                case "Thickness": return ControlsCreatorAvalonia.CreateEditBoxForThickness(propertyInfo, xmlControl);
                case "HorizontalAlignment": return ControlsCreatorAvalonia.CreateEditableControlForEnum(propertyInfo, xmlControl);
                case "VerticalAlignment": return ControlsCreatorAvalonia.CreateEditableControlForEnum(propertyInfo, xmlControl);
                case "FontWeight": return ControlsCreatorAvalonia.CreateEditableControlForEnum(propertyInfo, xmlControl);
                case "FontStyle": return ControlsCreatorAvalonia.CreateEditableControlForEnum(propertyInfo, xmlControl);
                case "IBrush": return ControlsCreatorAvalonia.CreateEditableControlForIBrush(propertyInfo, xmlControl);
                case "FontFamily": return ControlsCreatorAvalonia.CreateEditableControlForFontFamily(propertyInfo, xmlControl);
            }
        }
        else if(options == EditBoxOptions.EmbedBindingsView)
        {
            throw new NotImplementedException("Binding are not supported in this Version.");
        }
        return new TextBlock()
        {
            Text = "Can not load a Template for: " + propertyInfo.Name,
            Margin = new Thickness(0,5,0,0),
            Foreground = Brushes.Orange
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
        StackPanel stackPanel = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 5, 0, 0),
        };
        stackPanel.Children.Add(new TextBlock()
        {
            Text = propertyInfo.Name + ":",
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0 ,0 ,5 ,0)
        });
        stackPanel.Children.Add(control);
        return stackPanel;
    }
    
    private static Control CreateEditBoxForBoolean(PropertyInfo propertyInfo, XmlControl xmlControl)
    {
        CheckBox cb = new CheckBox()
        {
            Content = propertyInfo.Name,
            IsChecked = (bool)propertyInfo.GetValue(xmlControl.Control, null)!,
            IsEnabled = propertyInfo.CanWrite
        };
       
        cb.IsCheckedChanged += (s, e) =>
        {
            if (EditorController != null)
            {
                EditorController.OnPropertyInPropertiesViewChanged(xmlControl, propertyInfo, cb.IsChecked);
            }
        };
        return cb;
    }
    /// <summary>
    /// External Method which is an extraction for all Integer Based Types
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="xmlControl"></param>
    /// <returns></returns>
    private static Control CreateEditBoxForInteger(PropertyInfo propertyInfo, XmlControl xmlControl)
    {
        NumericUpDown numericUpDown = new NumericUpDown()
        {
            Text = propertyInfo.Name,
            FormatString = "0",
            Minimum = 0,
            Value = Convert.ToDecimal(propertyInfo.GetValue(xmlControl.Control, null)),
            Increment = 1,
        };
        numericUpDown.ValueChanged += (sender, args) =>
        {
            if (EditorController != null)
            {
                EditorController.OnPropertyInPropertiesViewChanged(xmlControl, propertyInfo, numericUpDown.Value);
            }
        };
        
        return AddEditBoxToStackPanel(numericUpDown, propertyInfo);
    }
    /// <summary>
    /// External Method which is an extraction for all Integer Based Types
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="xmlControl"></param>
    /// <returns></returns>
    private static Control CreateEditBoxForFloatingPoint(PropertyInfo propertyInfo, XmlControl xmlControl)
    {
        decimal value = 0;
        if (propertyInfo.PropertyType == typeof(float))
        {
            if(float.IsNaN((float)propertyInfo.GetValue(xmlControl.Control)))
            {
                return new TextBlock()
                {
                    Text = "Property: " + propertyInfo.Name + " is NaN",
                };
            }
            else
            {
                value = (decimal)propertyInfo.GetValue(xmlControl.Control)!;
            }    
        }
        else if (propertyInfo.PropertyType == typeof(double))
        {
            if(double.IsNaN((double)propertyInfo.GetValue(xmlControl.Control)))
            {
                return new TextBlock()
                {
                    Text = "Property: " + propertyInfo.Name + " is NaN",
                };
            }
            else
            {
                value = Convert.ToDecimal(propertyInfo.GetValue(xmlControl.Control));
            }    
        }
        
        NumericUpDown numericUpDown = new NumericUpDown()
        {
            Text = propertyInfo.Name,
            FormatString = "0.00",
            Minimum = 0,
            Value = value,
            Increment = 1,
        };
        numericUpDown.ValueChanged += (s, e) =>
        {
            if (EditorController != null)
            {
                EditorController.OnPropertyInPropertiesViewChanged(xmlControl, propertyInfo, numericUpDown.Value);
            }
        };
        return AddEditBoxToStackPanel(numericUpDown, propertyInfo);
    }
    /// <summary>
    /// External Method which is an extraction for all Integer Based Types
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="xmlControl"></param>
    /// <returns></returns>
    private static Control CreateEditBoxForString(PropertyInfo propertyInfo, XmlControl xmlControl)
    {
        TextBox tb = new TextBox()
        {
            Text = (string)propertyInfo.GetValue(xmlControl.Control, null)!,
            IsEnabled = propertyInfo.CanWrite,
        };
        tb.TextChanged += (s, e) =>
        {
            if (EditorController != null)
            {
                EditorController.OnPropertyInPropertiesViewChanged(xmlControl, propertyInfo, tb.Text);
            }
        };
        // EditorController.OnPropertyInPropertiesViewChanged(xmlControl, propertyInfo.Name, tb.Text);
        return AddEditBoxToStackPanel(tb, propertyInfo);
    }
}