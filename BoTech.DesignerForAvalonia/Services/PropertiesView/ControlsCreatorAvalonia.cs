using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Labs.Controls;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using BoTech.DesignerForAvalonia.Views;
using BoTech.DesignerForAvalonia.Controller.Editor;
using BoTech.DesignerForAvalonia.Models.XML;


namespace BoTech.DesignerForAvalonia.Services.PropertiesView;
/// <summary>
/// All editable Controls which can manipulate a Type from Avalonia can be created here.
/// Note this class is not static because it uses Bindings
/// </summary>
public static class ControlsCreatorAvalonia
{

    public static Control CreateEditableControlForIBrush(PropertyInfo propertyInfo, XmlControl xmlControl)
    {
       Border border = new Border()
       {
           CornerRadius = new CornerRadius(5),
           BorderThickness = new Thickness(2),
           BorderBrush = new SolidColorBrush(Colors.Gray),
       };
       ThemeEditor.Controls.ColorPicker.ColorPicker colorPicker = new ThemeEditor.Controls.ColorPicker.ColorPicker();
       try
       {
           SolidColorBrush? brush = Convert.ChangeType(propertyInfo.GetValue(xmlControl.Control), typeof(SolidColorBrush)) as SolidColorBrush;
           if (brush != null)
           {
               colorPicker.Color = brush.Color;
           }
       }
       catch (Exception ex)
       {
           colorPicker.Color = Colors.Green;
           Console.WriteLine("Can not set the Default Color for the ColorPicker: " + ex);
       }

       colorPicker.PropertyChanged += (s, e) =>
       {
           if (e.Property.Name == "Color")
           {
               try
               {
                   Color color = (Color)Convert.ChangeType(e.NewValue, typeof(Color));
                   if (color != null)
                   {
                       SolidColorBrush? brush = new SolidColorBrush(color);
                       EditorController.OnPropertyInPropertiesViewChanged(xmlControl, propertyInfo, brush);
                   }
               }
               catch (Exception exception)
               {
                   Console.WriteLine(exception);
               }
           }
       };
       border.Child = ControlsCreator.AddEditBoxToStackPanel(colorPicker, propertyInfo);
       return border;
    }

    public static Control CreateEditableControlForFontFamily(PropertyInfo propertyInfo, XmlControl xmlControl)
    {
        Dictionary<string, string> availableFontResources = new Dictionary<string, string>();
        XmlControl mostParentXmlControl = xmlControl.GetMostParent();
        if (mostParentXmlControl.Children.Count == 1)
        {
            XmlControl userControlXmlControl = mostParentXmlControl.Children[0];
            if (userControlXmlControl.Control.GetType().Name == "UserControl")
            {
                XmlNode? userControlXmlNode = GetXmlChildNodeByName("UserControl.Resources", userControlXmlControl.Node);
                // Search for a FontFamily Node
                if (userControlXmlNode != null)
                {
                    foreach (XmlNode childNode in userControlXmlNode.ChildNodes)
                    {
                        if (childNode.Name == "FontFamily" && childNode.Attributes != null)
                        {
                            string? key = childNode.Attributes["x:Key"]?.Value;
                            string path = childNode.InnerText;
                            if(key != null) availableFontResources.Add(key, path);
                        }
                    }  
                }
            }
        }
        
        // Converting the Dictionary to a List of strings
        List<string> fontFamilyNames = new List<string>();
        foreach (KeyValuePair<string, string> fontFamily in availableFontResources)
        {
            fontFamilyNames.Add(fontFamily.Key + " | " + fontFamily.Value);
        }
        
        
        
        // Creating AutoComplete Box:
        AutoCompleteBox autoCompleteBox = new AutoCompleteBox()
        {
            ItemsSource = fontFamilyNames,
            Margin = new Thickness(5,0,0,0),
        };
        autoCompleteBox.SelectionChanged += (s, e) =>
        {
            //OnFontFamilyChanged(xmlControl.Control, propertyInfo, availableFontResources, e);
        };

        StackPanel stackPanel = new StackPanel()
        {
            Children =
            {
                autoCompleteBox,
                new Label()
                {
                   
                    Classes = { "Purple" },
                    Content = "Beta",
                }
            },
            Orientation = Orientation.Horizontal
        };
        
        return ControlsCreator.AddEditBoxToStackPanel(stackPanel, propertyInfo);
    }

  /*  private static void OnFontFamilyChanged(Control control, PropertyInfo propertyInfo, Dictionary<string, string> availableFontResources, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0)
        {
            if (e.AddedItems[0] != null)
            {
                string[] selectedString = e.AddedItems[0].ToString().Split(" ");
                string resource = availableFontResources[selectedString[0]];
                FontFamily fontfamily = new FontFamily(resource);
                propertyInfo.SetValue(control, fontfamily);
            }
        }
    }*/
    private static XmlNode? GetXmlChildNodeByName(string nodeName, XmlNode node)
    {
        foreach (XmlNode childNode in node.ChildNodes)
        {
            if (childNode.Name == nodeName)
            {
                return childNode;
            }
        }
        return null;
    }

    /// <summary>
    /// This Method creates an ComboBox for all Enums. When the Selection changed the given Property of the Control will change too.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="xmlControl"></param>
    /// <returns></returns>
    public static Control CreateEditableControlForEnum(PropertyInfo propertyInfo, XmlControl xmlControl)
    {
        ComboBox choices = AddComboBoxItemsForEnum(propertyInfo, propertyInfo.GetValue(xmlControl.Control).ToString(), new ComboBox());
        choices.SelectionChanged += (s, e) =>
        {
            HandleSelectionForEnumChanged(propertyInfo, xmlControl, choices);
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
    private static ComboBox AddComboBoxItemsForEnum(PropertyInfo propertyInfo, string selectedItem, ComboBox comboBox)
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
    /// <param name="xmlControl"></param>
    /// <param name="comboBox"></param>
    private static void HandleSelectionForEnumChanged(PropertyInfo propertyInfo, XmlControl xmlControl, ComboBox comboBox)
    {
        ComboBoxItem selectedItem = ((ComboBoxItem)comboBox.SelectedItem!);
        propertyInfo.SetValue(xmlControl.Control, Enum.Parse(propertyInfo.PropertyType, selectedItem.Content.ToString()));
    }
    /// <summary>
    /// Creates a Control for the Thickness object. This Method can be used for Properties like Margin or Padding.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="control"></param>
    /// <returns></returns>
    public static Control CreateEditBoxForThickness(PropertyInfo propertyInfo, XmlControl xmlControl)
    {
        NumericUpDown nUdTop = new NumericUpDown()
        {
            Minimum = 0,
            Value = Convert.ToDecimal(((Thickness)propertyInfo.GetValue(xmlControl.Control)!).Top),
        };
        NumericUpDown nUdRight = new NumericUpDown()
        {
            Minimum = 0,
            Value = Convert.ToDecimal(((Thickness)propertyInfo.GetValue(xmlControl.Control)!).Right),
        };
        NumericUpDown nUdLeft = new NumericUpDown()
        {
            Minimum = 0,
            Value = Convert.ToDecimal(((Thickness)propertyInfo.GetValue(xmlControl.Control)!).Left),
        };
        NumericUpDown nUdBottom = new NumericUpDown()
        {
            Minimum = 0,
            Value = Convert.ToDecimal(((Thickness)propertyInfo.GetValue(xmlControl.Control)!).Bottom),
        };
        nUdTop.ValueChanged += (s, e) =>
        {
            HandleValueChangedEventForThickness(propertyInfo, xmlControl, nUdTop, nUdRight, nUdLeft, nUdBottom);
        };
        nUdRight.ValueChanged += (s, e) =>
        {
            HandleValueChangedEventForThickness(propertyInfo, xmlControl, nUdTop, nUdRight, nUdLeft, nUdBottom);
        };
        nUdLeft.ValueChanged += (s, e) =>
        {
            HandleValueChangedEventForThickness(propertyInfo, xmlControl, nUdTop, nUdRight, nUdLeft, nUdBottom);
        };
        nUdBottom.ValueChanged += (s, e) =>
        {
            HandleValueChangedEventForThickness(propertyInfo, xmlControl, nUdTop, nUdRight, nUdLeft, nUdBottom);
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
    /// Method first creates an new Thickness object and calls then the EditorController
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="xmlControl"></param>
    /// <param name="numericUpDownTop"></param>
    /// <param name="numericUpDownRight"></param>
    /// <param name="numericUpDownLeft"></param>
    /// <param name="numericUpDownBottom"></param>
    private static void HandleValueChangedEventForThickness(PropertyInfo propertyInfo, XmlControl xmlControl, NumericUpDown numericUpDownTop, NumericUpDown numericUpDownRight, NumericUpDown numericUpDownLeft, NumericUpDown numericUpDownBottom)
    {
        double top = Convert.ToDouble(numericUpDownTop.Value);
        double left = Convert.ToDouble(numericUpDownLeft.Value);
        double right = Convert.ToDouble(numericUpDownRight.Value);
        double bottom = Convert.ToDouble(numericUpDownBottom.Value);
        Thickness thickness = new Thickness(left, top, right, bottom);
        EditorController.OnPropertyInPropertiesViewChanged(xmlControl, propertyInfo, thickness);
    }
}