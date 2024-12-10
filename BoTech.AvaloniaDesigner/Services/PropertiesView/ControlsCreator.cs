using System;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace BoTech.AvaloniaDesigner.Services.PropertiesView;

/// <summary>
/// This class is a helper Class to build up the Properties View.
/// It creates Standardized Editatable Boxes for the Properties of an Control.
/// </summary>

public static class ControlsCreator
{
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
            if (options == EditBoxOptions.Auto)
            {
                switch (propertyInfo.PropertyType.Name)
                {
                    // All possible bool based Types:
                    case "Boolean":
                        CheckBox cb = new CheckBox()
                        {
                            Content = propertyInfo.Name,
                            IsChecked = (bool)propertyInfo.GetValue(control, null)!,
                            IsEnabled = propertyInfo.CanWrite
                        };
                        return cb;
                    case "bool":
                        CheckBox cb1 = new CheckBox()
                        {
                            Content = propertyInfo.Name,
                            IsChecked = (bool)propertyInfo.GetValue(control, null)!,
                            IsEnabled = propertyInfo.CanWrite
                        };
                        return cb1;
                        
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
                            Text = "Can not display: " + property + "because its +|- infinite.",
                        };
                    case "Double":
                        if ((double)propertyInfo.GetValue(control) != double.PositiveInfinity && (double)propertyInfo.GetValue(control) != double.NegativeInfinity)
                        {
                            return CreateEditBoxForFloatingPoint(propertyInfo, control);
                        }
                        return new TextBlock()
                        {
                            Text = "Can not display: " + property + "because its +|- infinite.",
                        };
                    case "Decimal": return CreateEditBoxForFloatingPoint(propertyInfo, control);
                            
                            
                    // Primitive Type string and Class String are handled the same way:
                    case "string": return CreateEditBoxForString(propertyInfo, control);
                    case "String": return CreateEditBoxForString(propertyInfo, control);
                }
            }
            else if(options == EditBoxOptions.EmbedBindingsView)
            {
                    
            }
        }
        //}
        return new TextBlock()
        {
            Text = "Can not load a Template for: " + property,
        };
    }
    /// <summary>
    /// External Method which is an extraction for all Integer Based Types
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    private static Control CreateEditBoxForInteger(PropertyInfo propertyInfo, Control control)
    {
        StackPanel panel = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            Children =
            {
                new TextBlock()
                {
                    Text = propertyInfo.Name + ":",
                    VerticalAlignment = VerticalAlignment.Center,
                },
                new NumericUpDown()
                {
                    Text = propertyInfo.Name,
                    FormatString = "0",
                    Minimum = 0,
                    Value = Convert.ToDecimal(propertyInfo.GetValue(control, null)),
                    Increment = 1,
                }
            }
        };
        return panel;
    }
    /// <summary>
    /// External Method which is an extraction for all Integer Based Types
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    private static Control CreateEditBoxForFloatingPoint(PropertyInfo propertyInfo, Control control)
    {
        StackPanel panel = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            Children =
            {
                new TextBlock()
                {
                    Text = propertyInfo.Name + ":",
                    VerticalAlignment = VerticalAlignment.Center,
                },
                new NumericUpDown()
                {
                    Text = propertyInfo.Name,
                    FormatString = "0.00",
                    Minimum = 0,
                    Value = Convert.ToDecimal(propertyInfo.GetValue(control, null))!,
                    Increment = 1,
                }
            }
        };
        return panel;
    }
    /// <summary>
    /// External Method which is an extraction for all Integer Based Types
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    private static Control CreateEditBoxForString(PropertyInfo propertyInfo, Control control)
    {
        StackPanel panel = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            Children =
            {
                new TextBlock()
                {
                    Text = propertyInfo.Name + ":",
                    VerticalAlignment = VerticalAlignment.Center,
                },
                new TextBox()
                {
                    Text = (string)propertyInfo.GetValue(control, null)!,
                    IsEnabled = propertyInfo.CanWrite
                }
            }
        };
        return panel;
    }
    
}