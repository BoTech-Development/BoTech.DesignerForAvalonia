using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using Avalonia.Controls;
using BoTech.AvaloniaDesigner.Models.XML;
using BoTech.AvaloniaDesigner.Services.Avalonia;

namespace BoTech.AvaloniaDesigner.Services.XML;

public class Deserializer
{
    public XmlNode RootNode { get; set; }
    public XmlControl RootConnectedNode { get; set; }
    private List<TypeInfo> AllControlTypes { get; set; }
    public Deserializer()
    {
        AllControlTypes = Assembly.Load(new AssemblyName("Avalonia.Controls")).DefinedTypes.ToList();
      /*  Deserialize(
            "<UserControl xmlns=\"https://github.com/avaloniaui\"\n             xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n             xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\"\n             xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"\n             xmlns:vm=\"clr-namespace:ADTest.ViewModels\"\n             mc:Ignorable=\"d\" d:DesignWidth=\"800\" d:DesignHeight=\"450\"\n             x:Class=\"ADTest.Views.MainView\"\n             x:DataType=\"vm:MainViewModel\">\n  <Design.DataContext>\n    <!-- This only sets the DataContext for the previewer in an IDE,\n         to set the actual DataContext for runtime, set the DataContext property in code (look at App.axaml.cs) -->\n    <vm:MainViewModel />\n  </Design.DataContext>\n\n  <TextBlock Text=\"{Binding Greeting}\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Center\"/>\n</UserControl>",
            null);
        */
    }
    /// <summary>
    /// Deserialize the Content of an axaml File into Controls and XmlControls to connect the Controls with the Xml Nodes.
    /// This Method stores the RootNode and the RootXmlControl in the RootConnectedControl Properties.
    /// </summary>
    /// <param name="xml"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public Control Deserialize(string xml, Assembly assembly)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        RootNode = doc.DocumentElement;
        RootConnectedNode = new XmlControl()
        {
            Node = null,
            Control = null
        };
        
        Control control = TranslateToControls(RootNode, RootConnectedNode);
        RootConnectedNode = RootConnectedNode.Children[0];
        return control;
    }

    private Control TranslateToControls(XmlNode currentNode, XmlControl parentXmlControl)
    {
        XmlControl currentXmlControl;
        Control control;
        TypeInfo? type = null;
        if ((type = AllControlTypes.Find(t => t.Name == currentNode.Name)) != null)
        {
            if ((control = (Control)Activator.CreateInstance(type)) == null)
            {
                control = new TextBlock()
                {
                    Text = "Activator Class returned null by creating a new Instance for Type: " + currentNode.Name
                };
            }
        }
        else
        {
            control = new TextBlock()
            {
                Text = "Can not find: " + currentNode.Name + " int the Avalonia.Controls dll."
            };
        }
        // Create a new XmlControl to connect the XmlNode to the Control
        currentXmlControl = new XmlControl()
        {
            Parent = parentXmlControl,
            Control = control,
            Node = currentNode,
        };
        parentXmlControl.Children.Add(currentXmlControl);
       

        AddPropertiesToControl(currentNode, control);
     
        PropertyInfo? propertyInfo = null;
        // There only be can one Child for the Child Property
        if ((propertyInfo = control.GetType().GetProperty("Child")) != null)
        {
            SetChildOrTextAsContent(propertyInfo, currentNode, control, currentXmlControl);
        }

        if ((propertyInfo = control.GetType().GetProperty("Children")) != null)
        {
            if (currentNode.ChildNodes.Count >= 1)
            {
                List<Control> children = new List<Control>();
                foreach (XmlNode child in currentNode.ChildNodes)
                {
                    children.Add(TranslateToControls(child, currentXmlControl));
                }
                if (propertyInfo.GetValue(control) is Controls childrenList)
                {
                    childrenList.AddRange(children);
                }
            }
            else if (currentNode.InnerText != string.Empty)
            {
                propertyInfo.SetValue(control, new TextBlock()
                {
                    Text = currentNode.InnerText,
                });
            }
        }

        if ((propertyInfo = control.GetType().GetProperty("Content")) != null)
        {
            // Beacuse User Controls may have the Design.DataContext Property Set, it is necessary to find the Layout Control in the ChildNodes.
            //  The Layout Control is the correct Control to work with
            if (control.GetType().Name == "UserControl")
            {
                XmlNode? xmlLayoutNode = null;
                foreach (XmlNode child in currentNode.ChildNodes)
                {
                    if (TypeCastingService.IsLayoutControl(child.Name))
                    {
                        xmlLayoutNode = child;
                        propertyInfo.SetValue(control, TranslateToControls(xmlLayoutNode, currentXmlControl));
                        break;
                    }
                }
                if(xmlLayoutNode == null)
                    SetChildOrTextAsContent(propertyInfo, currentNode, control, currentXmlControl);
            }
            else
            {
                SetChildOrTextAsContent(propertyInfo, currentNode, control, currentXmlControl);
            }
        }

        if ((propertyInfo = control.GetType().GetProperty("Text")) != null)
        {
            if (currentNode.InnerText != string.Empty)
            {
                propertyInfo.SetValue(control, currentNode.InnerText);
            }
        }
        
        return control;
    }

    private void SetChildOrTextAsContent(PropertyInfo propertyInfo, XmlNode current, Control control, XmlControl currentXmlControl)
    {
        if (current.ChildNodes.Count == 1)
        {
            propertyInfo.SetValue(control, TranslateToControls(current.ChildNodes[0], currentXmlControl));
        }
        else if(current.InnerText != string.Empty)
        {
            propertyInfo.SetValue(control, new TextBlock()
            {
                Text = current.InnerText,
            });
        }
    }
    /// <summary>
    /// Adds all Properties but no Bindings to the Control, that the Parser has read before.
    /// </summary>
    /// <param name="current"></param>
    /// <param name="control"></param>
    private void AddPropertiesToControl(XmlNode current, Control control)
    {
        foreach (XmlAttribute property in current.Attributes)
        {
            PropertyInfo? propertyInfo = null;
            // Check if Property is available
            if ((propertyInfo = control.GetType().GetProperty(property.Name)) != null)
            {
                if (propertyInfo.CanWrite)
                {
                    // When it is an enum
                    if (propertyInfo.PropertyType.IsEnum)
                    {
                        propertyInfo.SetValue(control, Enum.Parse(propertyInfo.PropertyType, property.Value));
                    }
                    else
                    {
                        propertyInfo.SetValue(control, Convert.ChangeType(property.Value, propertyInfo.PropertyType));    
                    }
                }
            }
        }
    }
    
}