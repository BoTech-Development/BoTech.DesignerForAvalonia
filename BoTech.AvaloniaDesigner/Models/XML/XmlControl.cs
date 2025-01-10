using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Avalonia.Controls;

namespace BoTech.AvaloniaDesigner.Models.XML;
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
            if (this.Node.Attributes != null)
            {
                foreach (XmlAttribute attribute in this.Node.Attributes)
                {
                    PropertyInfo? propertyInfo = copiedControl.GetType().GetProperty(attribute.Name);
                    if (propertyInfo != null)
                    {
                        if (propertyInfo.PropertyType.IsEnum)
                        {
                            propertyInfo.SetValue(copiedControl,
                                Enum.Parse(propertyInfo.PropertyType, attribute.Value));
                        }
                        else
                        {
                            propertyInfo.SetValue(copiedControl,
                                Convert.ChangeType(attribute.Value, propertyInfo.PropertyType));
                        }
                    }
                }
            }
            xmlControlCopy.Control = copiedControl;
        }
        
        // Creating a Copy of the Node
        
        XmlElement? newNode = Node.OwnerDocument?.CreateElement(Control.GetType().Name);
        if (newNode != null && Node.Attributes != null)
        {
            // Adding all Attributes:
            foreach (XmlAttribute attribute in Node.Attributes)
            {
                newNode.Attributes.Append(attribute);
            }
            xmlControlCopy.Node = newNode;
        }
        
        return xmlControlCopy;
    }
}