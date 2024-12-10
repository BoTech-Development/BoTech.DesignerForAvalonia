using System;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;

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
    /// <returns>One Avalonia Control</returns>
    public static Control CreateEditBox(Control control, string property, EditBoxOptions options)
    {
        Type controlType = control.GetType();
        if (controlType.BaseType != typeof(Control))
        {
            PropertyInfo[] propertyInfos = controlType.GetProperties();
            PropertyInfo? propertyInfo;
            if ((propertyInfo = propertyInfos.Where(p => p.Name == property).FirstOrDefault()) != null)
            {
                if (options == EditBoxOptions.Auto)
                {
                    switch (propertyInfo.PropertyType.Name)
                    {
                        case "Boolean":
                            //binding = new Subject<T>();
                            CheckBox cb = new CheckBox()
                            {
                                Content = propertyInfo.Name,
                                IsChecked = (bool)propertyInfo.GetValue(control, null)!,
                                IsEnabled = propertyInfo.CanWrite
                            };
                            //cb.Bin)
                            return cb;
                        case "int":
                            NumericUpDown numInt = new NumericUpDown()
                            {
                                Text = propertyInfo.Name,
                                FormatString = "0",
                                Minimum = 0,
                                Value = (decimal)propertyInfo.GetValue(control, null)!,
                                Increment = 1,
                            };
                            return numInt;
                        case "Double":
                            NumericUpDown numDouble = new NumericUpDown()
                            {
                                Text = propertyInfo.Name,
                                FormatString = "0.00",
                                Minimum = 0,
                                Value = Convert.ToDecimal(propertyInfo.GetValue(control, null))!,
                                Increment = 1,
                            };
                            return numDouble;
                        case "string":
                            TextBox tb = new TextBox()
                            {
                                Text = (string)propertyInfo.GetValue(control, null)!,
                                IsEnabled = propertyInfo.CanWrite
                            };
                            return tb;
                    }
                }
                else if(options == EditBoxOptions.EmbedBindingsView)
                {
                    
                }
            }
        }
        return new TextBlock()
        {
            Text = "Can not load a Template for: " + property,
        };
    }
}