using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Layout;
using BoTech.AvaloniaDesigner.Models.XML;
using BoTech.AvaloniaDesigner.Services.Avalonia;

namespace BoTech.AvaloniaDesigner.Services.XML;

public class Serializer
{
    private const char OpenTag = '<';
    private const char EndTag = '/';
    private const char CloseTag = '>';
    private const char PropertyTag = '=';
    private const char OpenOrCloseValueTag = '"';
    private const char OpenBindingTag = '{';
    private const char CloseBindingTag = '}';
    private const char PrefixSeparatorTag = ':';
    private const char Spacer = ' ';
    private List<TypeInfo> _allAvaloniaControlTypes;
    
    public Serializer()
    {
        _allAvaloniaControlTypes = TypeCastingService.GetAllControlBasedAvaloniaTypes();
        StackPanel stackPanel = new StackPanel()
        {
            Orientation = Orientation.Vertical,
            Children =
            {
                new TextBlock()
                {
                    Text = "Hellord"
                },
                new Button()
                {
                    Content = "Click_Me!",
                }
            }
        };
        Serialize(stackPanel);
    }

    public string Serialize(Control control)
    {
        XmlObject xmlParent = TranslateControlToXmlObject(control, null);
        string result = TranslateXmlObjectToString(xmlParent);
        return result;
    }

    private string TranslateXmlObjectToString(XmlObject xmlObject, int recursionDepth = 0)
    {
        string result = "\n" + CreateTabSymbols(recursionDepth) + "<" + xmlObject.NameOfType;
        
        // All Properties
        foreach (XmlProperty property in xmlObject.Properties)
        {
            string[] invalidTypeNames = { "Content", "Child", "Children", "Text" };
            if(!invalidTypeNames.Contains(property.PropertyName))
                result += " " + property.PropertyName + "=\"" + property.PropertyValue + "\"";
        }

        if (xmlObject.Children.Count == 0 && xmlObject.DataBetween == string.Empty)
        {
            result += "/>";
            return result;
        }

        if (xmlObject.Children.Count >= 1)
        {
            result += ">";
            foreach (XmlObject child in xmlObject.Children)
            {
                result += TranslateXmlObjectToString(child);
            }
            result += "\n" + CreateTabSymbols(recursionDepth) + CreateTabSymbols(recursionDepth)  + "</" + xmlObject.NameOfType + ">";
            return result;
        }

        if(xmlObject.DataBetween != string.Empty)
        {
            result += ">\n" + CreateTabSymbols(recursionDepth + 1) + xmlObject.DataBetween + "\n" + CreateTabSymbols(recursionDepth) + "</" + xmlObject.NameOfType + ">";
            return result;
        }

        return "Error";
    }

    private string CreateTabSymbols(int tabCount)
    {
        string result = string.Empty;
        for (int i = 0; i < tabCount; i++)
        {
            result += "\t";
        }
        return result;
    }
    private XmlObject TranslateControlToXmlObject(Control control, XmlObject parent)
    {
        XmlObject current = new XmlObject()
        {
            NameOfType = control.GetType().Name,
            Parent = parent,
        };
        
        AddPropertiesThatChanged(current, control);
        
        PropertyInfo? propertyInfo = null;
        // Check which Property the Given Control has to create the Data Between Property
        if ((propertyInfo = control.GetType().GetProperty("Children")) != null)
        {
            object? value = propertyInfo.GetValue(control);
            if (value != null)
            {
                if (value is Controls children)
                {
                    foreach (Control child in children)
                    {
                        current.Children.Add(TranslateControlToXmlObject(child, current));
                    }
                }
            }
        }
        if ((propertyInfo = control.GetType().GetProperty("Child")) != null)
        {
            TranslateChildOrContentProperty(propertyInfo, current, control);
        }
        if ((propertyInfo = control.GetType().GetProperty("Content")) != null)
        {
            TranslateChildOrContentProperty(propertyInfo, current, control);
        }

        if ((propertyInfo = control.GetType().GetProperty("Text")) != null)
        {
            string? text = propertyInfo.GetValue(control)?.ToString();
            if(text != null)current.DataBetween = text;
        }
        return current;
    }

    private void AddPropertiesThatChanged(XmlObject current, Control control)
    {
        object? convertedControl = TryToChangeTypeToAvaloniaControl(control);
       
        if (convertedControl != null)
        {
            object? defaultControl = Activator.CreateInstance(convertedControl.GetType());
            if (defaultControl != null)
            {
                List<PropertyInfo> defaultProperties = defaultControl.GetType().GetProperties().ToList();
                List<PropertyInfo> properties = convertedControl.GetType().GetProperties().ToList();
                foreach (PropertyInfo property in properties)
                {
                    // Check if the Property is not the Default Value
                    if (property.CanRead)
                    {
                        // Find the equivalent property to the "property" in the defaultProperty List
                        PropertyInfo? defaultProperty = defaultProperties.FirstOrDefault(p => p.Name == property.Name);
                        if (defaultProperty != null)
                        {
                            if (defaultProperty.CanRead)
                            {
                                try
                                {
                                    if (defaultProperty.GetValue(defaultControl) != property.GetValue(control))
                                    {
                                        XmlProperty xmlProperty = new XmlProperty()
                                        {
                                            PropertyName = property.Name,
                                            PropertyValue = property.GetValue(convertedControl).ToString(),
                                        };
                                        current.Properties.Add(xmlProperty);
                                    }
                                }
                                catch (Exception e)
                                {
                                    // GetValue can throw an TargetParameterCountException
                                    Console.WriteLine(e);
                                    
                                }
                              
                            }
                        }
                    }
                }
            }
        }
    }

    private object? TryToChangeTypeToAvaloniaControl(Control control)
    {
        foreach (TypeInfo typeInfo in _allAvaloniaControlTypes)
        {
            try
            {
                object? changedType = Convert.ChangeType(control, typeInfo);
                if(changedType != null) return changedType;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
           
        }
        return null;
    }
    private void TranslateChildOrContentProperty(PropertyInfo propertyInfo, XmlObject current, Control control)
    {
        object? content = propertyInfo.GetValue(control, null);
        if (content != null)
        {
            // Data can be set without another xml Node between.
            if (content is TextBlock textBlock)
            {
                if (textBlock.Text != null) current.DataBetween = textBlock.Text;
            }
            // The Data Between is another Control so it is necessary to create another xml node.
            else if (content is Control childControl)
            {
                current.Children.Add(TranslateControlToXmlObject(childControl, current));
            }
        }
    }
}