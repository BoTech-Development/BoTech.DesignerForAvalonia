using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Avalonia.Controls;
using Avalonia.Layout;
using BoTech.DesignerForAvalonia.Views;
using BoTech.DesignerForAvalonia.Models.XML;
using BoTech.DesignerForAvalonia.Services.Avalonia;
using BoTech.DesignerForAvalonia.ViewModels;
using DialogHostAvalonia;

namespace BoTech.DesignerForAvalonia.Services.XML;

public class Serializer
{
    private List<TypeInfo> _allAvaloniaControlTypes;
    /// <summary>
    /// The Loading View for the Message Box
    /// </summary>
    private LoadingViewModel _loadingViewModel;
    public Serializer(LoadingViewModel loadingViewModel)
    {
        _allAvaloniaControlTypes = TypeCastingService.GetAllControlBasedAvaloniaTypes();
        _loadingViewModel = loadingViewModel;
    }

    public string Serialize(XmlControl xmlControl)
    {
        _loadingViewModel.Maximum = CountAllChildren(xmlControl);
        _loadingViewModel.IsIndeterminate = false;
        UpdateXmlNodesForControl(xmlControl);
        string result = SerializeXmlNode(xmlControl.Node);
        return result;
    }
    public string SerializeXmlNode(XmlNode node)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(XmlNode));//, new XmlRootAttribute(node.Name));
       // TextWriter writer = new Stream
        
        using (StringWriter stringWriter = new StringWriter())
        {
            serializer.Serialize(stringWriter, node);
            string result = stringWriter.ToString();
            return result.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n", "");
        }
    }
    private void UpdateXmlNodesForControl(XmlControl current)
    {
        _loadingViewModel.Current += 1;
        _loadingViewModel.SubStatusText = "Updating Xml Node: " + current.Node.Name;
        
        //AddPropertiesThatChanged(current.Node, current.Control);
        // When the current XmlControl has more than one Children => it must be a Control which has the Children Property. 
        if (current.Children.Count > 1)
        {
            foreach (XmlControl child in current.Children)
            {
                UpdateXmlNodesForControl(child);
            }
        }
        else
        {
            PropertyInfo? propertyInfo = null;
            if ((propertyInfo = current.Control.GetType().GetProperty("Child")) != null)
            {
                TranslateChildOrContentProperty(propertyInfo, current, current.Control);
            }

            if ((propertyInfo = current.Control.GetType().GetProperty("Content")) != null)
            {
                TranslateChildOrContentProperty(propertyInfo, current, current.Control);
            }

            if ((propertyInfo = current.Control.GetType().GetProperty("Text")) != null)
            {
                string? text = propertyInfo.GetValue(current.Control)?.ToString();
                if (text != null) current.Node.InnerText = text;
            }
        }
    }
    /// <summary>
    /// Checks which Property of a Control has not the default Value and creates new Attribute for this Node.
    /// </summary>
    /// <param name="current"></param>
    /// <param name="control"></param>
    [Obsolete("All Properties are added during the drop of a new Control.")]
    private void AddPropertiesThatChanged(XmlNode current, Control control)
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
                        PropertyInfo? defaultProperty = defaultProperties.Find(p => p.Name == property.Name);
                        if (defaultProperty != null)
                        {
                            if (defaultProperty.CanRead)
                            {
                                try
                                {
                                    if (defaultProperty.GetValue(defaultControl) != property.GetValue(control))
                                    {
                                        XmlAttribute attribute = current.OwnerDocument.CreateAttribute(property.Name);
                                        attribute.Value = property.GetValue(control).ToString();
                                        current.Attributes.Append(attribute);
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
    /// <summary>
    /// This Method is needed to get the correct Type. For example when the given Control is a TextBlock the Method will return the TextBlock.
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
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
    /// <summary>
    /// This Method can be used to set the InnerText Property of an XmlNode.
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="current"></param>
    /// <param name="control"></param>
    private void TranslateChildOrContentProperty(PropertyInfo propertyInfo, XmlControl current, Control control)
    {
        object? content = propertyInfo.GetValue(control, null);
        if (content != null)
        {
            // Data can be set without another xml Node between.
            if (content is TextBlock textBlock)
            {
                if (textBlock.Text != null) current.Node.InnerText = textBlock.Text;
            }
            // The Data Between is another Control so it is necessary to create another xml node.
            else if (content is Control childControl)
            {
                UpdateXmlNodesForControl(current.Children[0]);
                //current.Children.Add(UpdateXmlNodesForControl(current));
            }
        }
    }
    /// <summary>
    /// This Method counts all Children of the xmlControl
    /// </summary>
    /// <param name="xmlControl"></param>
    /// <returns></returns>
    private int CountAllChildren(XmlControl xmlControl)
    {
        int count = xmlControl.Children.Count;
        foreach (XmlControl child in xmlControl.Children)
        {
            count += CountAllChildren(child);
        }
        return count;
    }
}