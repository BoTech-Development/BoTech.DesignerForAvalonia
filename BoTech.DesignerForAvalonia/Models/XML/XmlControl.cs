using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Avalonia.Controls;
using BoTech.DesignerForAvalonia.Services.Avalonia;

namespace BoTech.DesignerForAvalonia.Models.XML;
/// <summary>
/// This Model connects an XmlNode with a Control.
/// This is necessary for the serialization Process where old XmlNodes may be edited.
/// </summary>
public class XmlControl : ICloneable
{
    public XmlControl? Parent { get; set; }
    public List<XmlControl> Children { get; set; } = new List<XmlControl>();
    public  Control Control { get; set; }
    public  XmlNode Node { get; set; }
    /// <summary>
    /// Searches in every Child for the given Control and returns itself.
    /// </summary>
    /// <param name="control"></param>
    /// <returns>Returns null when nothing found otherwise itself</returns>
    public XmlControl? Find(Control control)
    {
        if (control == Control)
        {
            return this;
        }
        XmlControl? xmlControl = null;
        foreach (XmlControl child in Children)
        {
            if ((xmlControl = child.Find(control)) != null)
            {
                return xmlControl;
            }
        }
        return null;
    }
    /// <summary>
    /// Searches in every Child for the given node and returns itself.
    /// </summary>
    /// <param name="node"></param>
    /// <returns>Returns null when nothing found otherwise itself.</returns>
    public XmlControl Find(XmlNode node)
    {
        if (node == Node)
        {
            return this;
        }
        XmlControl? xmlControl = null;
        foreach (XmlControl child in Children)
        {
            if ((xmlControl = child.Find(node)) != null)
            {
                return xmlControl;
            }
        }
        return null;
    }
    /// <summary>
    /// Gets the most Parent Control.
    /// </summary>
    /// <returns></returns>
    public XmlControl GetMostParent()
    {
        if(Parent != null) return Parent.GetMostParent();
        return this;
    }
    /// <summary>
    /// Clone the object: Method clones all Properties of the Control and inject it into a new Control.
    /// It also only copies all Attributes of the XmlNode into a new XmlNode. 
    /// </summary>
    /// <returns></returns>
    public object Clone()
    {
        XmlControl xmlControlCopy = new XmlControl();
        
        // Create a "deep Copy" of the Control by using the XmlNode.
        
        Control? copiedControl = Activator.CreateInstance(this.Control.GetType()) as Control;
        if (copiedControl != null)
        {
            // We want to iterate through the Attribute List to only copy the Properties that have really changed. 
            if (this.Node.Attributes != null)
            {
                foreach (XmlAttribute attribute in this.Node.Attributes)
                {
                    PropertyInfo? propertyInfo = copiedControl.GetType().GetProperty(attribute.Name);
                    if (propertyInfo != null)
                    {
                        // Get the Property of the Current Control and Clone it
                        
                       // propertyInfo.SetValue(copiedControl, propertyInfo.GetValue(this.Node.Attributes[attribute.Name], null));
                        if (propertyInfo.PropertyType.IsEnum)
                        {
                            propertyInfo.SetValue(copiedControl,
                                Enum.Parse(propertyInfo.PropertyType, attribute.Value));
                        }
                        else
                        {
                            propertyInfo.SetValue(copiedControl, CloneService.CloneProperty(propertyInfo, copiedControl, Control));
                            
                        }
                    }
                }
            }
            xmlControlCopy.Control = copiedControl;
        }
        
        // Creating a Copy of the Node

        xmlControlCopy.Node  = Node.CloneNode(true);
        
        return xmlControlCopy;
    }
    
}