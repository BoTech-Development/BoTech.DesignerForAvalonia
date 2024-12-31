using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using Avalonia.Controls;
using BoTech.AvaloniaDesigner.Models.XML;

namespace BoTech.AvaloniaDesigner.Services.XML;

public class Deserializer
{
    public XmlNode RootNode { get; set; }
    
    private const char OpenTag = '<';
    private const char EndTag = '/';
    private const char CloseTag = '>';
    private const char PropertyTag = '=';
    private const char OpenOrCloseValueTag = '"';
    private const char OpenBindingTag = '{';
    private const char CloseBindingTag = '}';
    private const char PrefixSeparatorTag = ':';
    private const char Spacer = ' ';
    private const string OpenCommentTag = "<!--";
    private const string CloseCommentTag = "-->";
    
    private List<TypeInfo> AllControlTypes { get; set; }
    public Deserializer()
    {
        AllControlTypes = Assembly.Load(new AssemblyName("Avalonia.Controls")).DefinedTypes.ToList();
      /*  Deserialize(
            "<UserControl xmlns=\"https://github.com/avaloniaui\"\n             xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n             xmlns:d=\"http://schemas.microsoft.com/expression/blend/2008\"\n             xmlns:mc=\"http://schemas.openxmlformats.org/markup-compatibility/2006\"\n             xmlns:vm=\"clr-namespace:ADTest.ViewModels\"\n             mc:Ignorable=\"d\" d:DesignWidth=\"800\" d:DesignHeight=\"450\"\n             x:Class=\"ADTest.Views.MainView\"\n             x:DataType=\"vm:MainViewModel\">\n  <Design.DataContext>\n    <!-- This only sets the DataContext for the previewer in an IDE,\n         to set the actual DataContext for runtime, set the DataContext property in code (look at App.axaml.cs) -->\n    <vm:MainViewModel />\n  </Design.DataContext>\n\n  <TextBlock Text=\"{Binding Greeting}\" HorizontalAlignment=\"Center\" VerticalAlignment=\"Center\"/>\n</UserControl>",
            null);
        */
    }
    public Control Deserialize(string xml, Assembly assembly)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        RootNode = doc.DocumentElement;
        return TranslateToControls(RootNode);
        
       /* XmlObject parent = new XmlObject()
        {
            NameOfType = "Parent",
            Parent = null
        };
     /*   string oneLineXml = xml.Replace("\t", "").Replace("\n", "").Replace("\r", "");
        string newOneLineXml = string.Empty;
        bool foundSecond = false;
        for (int i = 0; i < oneLineXml.Length; i++)
        {
            if (foundSecond)
            {
                if (oneLineXml[i] != Spacer)
                {
                    foundSecond = false;
                    newOneLineXml += oneLineXml[i];
                }
            }
            else
            {
                if (oneLineXml[i] == Spacer)
                {
                    foundSecond = true;
                }
                // When the next Character is an <
                if(oneLineXml[i + 1] != OpenTag) newOneLineXml += oneLineXml[i];
                
            }
        }*/
        
        //ConvertXmlIntoNodes("<SP><TB Text=\"Hellord\"/><BTN>Click Me</BTN><SP Orientation=\"Horizontal\"><TB>Status:...</TB><BTN>Reset</BTN></SP></SP>", parent);
        //ConvertXmlIntoNodes("<StackPanel><TextBlock Text=\"Hellord\"/><Button>Click Me</Button><StackPanel><TextBlock>Status:...</TextBlock><Button>Reset</Button></StackPanel></StackPanel>", parent);
        /*ConvertXmlIntoNodes(xml, parent);
        ConvertXmlPropertiesInXmlBindings(parent);
        Control control = TranslateToControls(parent.Children[0]);
        return control;*/
    }

    private Control TranslateToControls(XmlNode current)
    {
        Control control;
        TypeInfo? type = null;
        if ((type = AllControlTypes.Find(t => t.Name == current.Name)) != null)
        {
            control = (Control)Activator.CreateInstance(type);
        }
        else
        {
            control = new TextBlock()
            {
                Text = "Can not find: " + current.Name + " int the Avalonia.Controls dll."
            };
        }
        AddPropertiesToControl(current, control);
     
        PropertyInfo? propertyInfo = null;
        // There only be can one Child for the Child Property
        if ((propertyInfo = control.GetType().GetProperty("Child")) != null)
        {
            SetChildOrTextAsContent(propertyInfo, current, control);
                
        }

        if ((propertyInfo = control.GetType().GetProperty("Children")) != null)
        {
            if (current.ChildNodes.Count >= 1)
            {
                List<Control> children = new List<Control>();
                foreach (XmlNode child in current.ChildNodes)
                {
                    children.Add(TranslateToControls(child));
                }
                if (propertyInfo.GetValue(control) is Controls childrenList)
                {
                    childrenList.AddRange(children);
                }
            }
            else if (current.InnerText != string.Empty)
            {
                propertyInfo.SetValue(control, new TextBlock()
                {
                    Text = current.InnerText,
                });
            }
        }

        if ((propertyInfo = control.GetType().GetProperty("Content")) != null)
        {
            SetChildOrTextAsContent(propertyInfo, current, control);
        }

        if ((propertyInfo = control.GetType().GetProperty("Text")) != null)
        {
            if (current.InnerText != string.Empty)
            {
                propertyInfo.SetValue(control, current.InnerText);
            }
        }
        
        return control;
    }

    private void SetChildOrTextAsContent(PropertyInfo propertyInfo, XmlNode current, Control control)
    {
        if (current.ChildNodes.Count == 1)
        {
            propertyInfo.SetValue(control, TranslateToControls(current.ChildNodes[0]));
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
                if(propertyInfo.CanWrite) propertyInfo.SetValue(control, Convert.ChangeType(property.Value, propertyInfo.PropertyType));
            }
        }
    }
    
}