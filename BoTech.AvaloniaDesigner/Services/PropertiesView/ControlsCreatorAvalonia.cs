using System;
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

    public static Control CreateEditableControlForAlignment(PropertyInfo propertyInfo, Control control)
    {
        StackPanel panel = new();
        panel.Orientation = Orientation.Horizontal;
        ComboBox choices = new ComboBox();
        if (propertyInfo.PropertyType == typeof(HorizontalAlignment))
        {
            choices = new ComboBox()
            {
                Items =
                {
                    new ComboBoxItem()
                    {
                        Content = "Center"
                    },
                    new ComboBoxItem()
                    {
                        Content = "Left"
                    },
                    new ComboBoxItem()
                    {
                        Content = "Right"
                    },
                    new ComboBoxItem()
                    {
                        Content = "Stretch"
                    }
                }
            };
        }
        else if (propertyInfo.PropertyType == typeof(VerticalAlignment))
        {
            choices = new ComboBox()
            {
                Items =
                {
                    new ComboBoxItem()
                    {
                        Content = "Bottom"
                    },
                    new ComboBoxItem()
                    {
                        Content = "Center"
                    },
                    new ComboBoxItem()
                    {
                        Content = "Top"
                    },
                    new ComboBoxItem()
                    {
                        Content = "Stretch"
                    }
                },
                SelectedValue =new ComboBoxItem()
                {
                    Content = propertyInfo.GetValue(control).ToString()
                } 
            };
        }

        choices.SelectionChanged += (s, e) =>
        {
            HandleSelectionChangedForAlignment(propertyInfo, control, choices);
        };
        panel.Children.Add(new TextBlock()
        {
            Text = propertyInfo.Name + ":"
        });
        panel.Children.Add(choices);
        return panel;
    }

    private static void HandleSelectionChangedForAlignment(PropertyInfo propertyInfo, Control control, ComboBox comboBox)
    {
        if (comboBox.SelectedItem != null && PreviewController != null)
        {
            if (propertyInfo.PropertyType == typeof(HorizontalAlignment))
            {
                switch (((ComboBoxItem)comboBox.SelectedItem).Content!)
                {
                    case "Center":
                        PreviewController.OnPropertyInPropertiesViewChanged(control, propertyInfo, HorizontalAlignment.Center);
                        break;
                    case "Left":
                        PreviewController.OnPropertyInPropertiesViewChanged(control, propertyInfo, HorizontalAlignment.Left);
                        break;
                    case "Right":
                        PreviewController.OnPropertyInPropertiesViewChanged(control, propertyInfo, HorizontalAlignment.Right);
                        break;
                    case "Stretch":
                        PreviewController.OnPropertyInPropertiesViewChanged(control, propertyInfo, HorizontalAlignment.Stretch);
                        break;
                }
            }
            else if (propertyInfo.PropertyType == typeof(VerticalAlignment))
            {
                switch (((ComboBoxItem)comboBox.SelectedItem).Content!)
                {
                    case "Bottom":
                        PreviewController.OnPropertyInPropertiesViewChanged(control, propertyInfo, VerticalAlignment.Bottom);
                        break;
                    case "Center":
                        PreviewController.OnPropertyInPropertiesViewChanged(control, propertyInfo, VerticalAlignment.Center);
                        break;
                    case "Top":
                        PreviewController.OnPropertyInPropertiesViewChanged(control, propertyInfo, VerticalAlignment.Top);
                        break;
                    case "Stretch":
                        PreviewController.OnPropertyInPropertiesViewChanged(control, propertyInfo, VerticalAlignment.Stretch);
                        break;
                }
            }
        }
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