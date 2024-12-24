using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using BoTech.AvaloniaDesigner.Controller.Editor;

namespace BoTech.AvaloniaDesigner.Services.PropertiesView;
/// <summary>
/// All editable Controls which can manipulate a Type from Avalonia can be created here.
/// Note this class is not static because it uses Bindings
/// </summary>
public static class ControlsCreatorAvalonia
{
    public static PreviewController? PreviewController { get; set; } = new();

    /// <summary>
    /// This Method creates an ComboBox for all Enums. When the Selection changed the given Property of the Control will change too.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    public static Control CreateEditableControlForEnum(PropertyInfo propertyInfo, Control control)
    {
        ComboBox choices = AddComboBoxItemsForEnum(propertyInfo, propertyInfo.GetValue(control).ToString(), new ComboBox());
        choices.SelectionChanged += (s, e) =>
        {
            HandleSelectionForEnumChanged(propertyInfo, control, choices);
        };
        return ControlsCreator.AddEditBoxToStackPanel(choices, propertyInfo);
    }
    
    /// <summary>
    /// A Helper Method to Create a ComboBox for an Enum. This Method also sets the selected Value of the ComboBox to the current Value of the propertyInfo (Control).
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="selectedItem"></param>
    /// <param name="comboBox"></param>
    /// <returns></returns>
    private static ComboBox AddComboBoxItemsForEnum(PropertyInfo propertyInfo, string selectedItem , ComboBox comboBox)
    {
        string[] items = Enum.GetNames(propertyInfo.PropertyType);
        foreach (string item in items)
        {
            ComboBoxItem comboBoxItem = new ComboBoxItem()
            {
                Content = item
            };
            comboBox.Items.Add(comboBoxItem);
            if (item == selectedItem)
            {
                comboBox.SelectedItem = comboBoxItem;
            }
        }
        return comboBox;
    }
    /// <summary>
    /// Event Handler for any enum based Property. This Method sets or changes the property in the Control.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="control"></param>
    /// <param name="comboBox"></param>
    private static void HandleSelectionForEnumChanged(PropertyInfo propertyInfo, Control control, ComboBox comboBox)
    {
        ComboBoxItem selectedItem = ((ComboBoxItem)comboBox.SelectedItem!);
        propertyInfo.SetValue(control, Enum.Parse(propertyInfo.PropertyType, selectedItem.Content.ToString()));
    }
    /// <summary>
    /// Creates a Control for the Thickness object. This Method can be used for Properties like Margin or Padding.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    public static Control CreateEditBoxForThickness(PropertyInfo propertyInfo, Control control)
    {
        NumericUpDown nUdTop = new NumericUpDown()
        {
            Minimum = 0,
            Value = Convert.ToDecimal(((Thickness)propertyInfo.GetValue(control)!).Top),
        };
        NumericUpDown nUdRight = new NumericUpDown()
        {
            Minimum = 0,
            Value = Convert.ToDecimal(((Thickness)propertyInfo.GetValue(control)!).Right),
        };
        NumericUpDown nUdLeft = new NumericUpDown()
        {
            Minimum = 0,
            Value = Convert.ToDecimal(((Thickness)propertyInfo.GetValue(control)!).Left),
        };
        NumericUpDown nUdBottom = new NumericUpDown()
        {
            Minimum = 0,
            Value = Convert.ToDecimal(((Thickness)propertyInfo.GetValue(control)!).Bottom),
        };
        nUdTop.ValueChanged += (s, e) =>
        {
            HandleValueChangedEventForThickness(propertyInfo, control, nUdTop, nUdRight, nUdLeft, nUdBottom);
        };
        nUdRight.ValueChanged += (s, e) =>
        {
            HandleValueChangedEventForThickness(propertyInfo, control, nUdTop, nUdRight, nUdLeft, nUdBottom);
        };
        nUdLeft.ValueChanged += (s, e) =>
        {
            HandleValueChangedEventForThickness(propertyInfo, control, nUdTop, nUdRight, nUdLeft, nUdBottom);
        };
        nUdBottom.ValueChanged += (s, e) =>
        {
            HandleValueChangedEventForThickness(propertyInfo, control, nUdTop, nUdRight, nUdLeft, nUdBottom);
        };
        
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
                new Border()
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(2), 
                    Child = new StackPanel()
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            new TextBlock()
                            {
                                Text = "Top:",
                                VerticalAlignment = VerticalAlignment.Center,
                                Margin = new Thickness(2),
                            },
                            nUdTop,
                            new TextBlock()
                            {
                                Text = "Left:",
                                VerticalAlignment = VerticalAlignment.Center,
                                Margin = new Thickness(2),
                            },
                            nUdLeft,
                            new TextBlock()
                            {
                                Text = "Right:",
                                VerticalAlignment = VerticalAlignment.Center,
                                Margin = new Thickness(2),
                            },
                            nUdRight,
                            new TextBlock()
                            {
                                Text = "Bottom:",
                                VerticalAlignment = VerticalAlignment.Center,
                                Margin = new Thickness(2),
                            },
                            nUdBottom  
                        } 
                    }
                }
            }
        };
        return panel;
    }
    /// <summary>
    /// Method first creates an new Thickness object and calls then the PreviewController
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="control"></param>
    /// <param name="numericUpDownTop"></param>
    /// <param name="numericUpDownRight"></param>
    /// <param name="numericUpDownLeft"></param>
    /// <param name="numericUpDownBottom"></param>
    private static void HandleValueChangedEventForThickness(PropertyInfo propertyInfo, Control control, NumericUpDown numericUpDownTop, NumericUpDown numericUpDownRight, NumericUpDown numericUpDownLeft, NumericUpDown numericUpDownBottom)
    {
        double top = Convert.ToDouble(numericUpDownTop.Value);
        double left = Convert.ToDouble(numericUpDownLeft.Value);
        double right = Convert.ToDouble(numericUpDownRight.Value);
        double bottom = Convert.ToDouble(numericUpDownBottom.Value);
        Thickness thickness = new Thickness(left, top, right, bottom);
        if(PreviewController != null) PreviewController.OnPropertyInPropertiesViewChanged(control, propertyInfo, thickness);
    }
}