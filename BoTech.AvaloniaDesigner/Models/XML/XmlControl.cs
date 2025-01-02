using System.Collections.Generic;
using System.Xml;
using Avalonia.Controls;

namespace BoTech.AvaloniaDesigner.Models.XML;
/// <summary>
/// This Model connects an XmlNode with a Control.
/// This is necessary for the serialization Process where old XmlNodes may be edited.
/// </summary>
public class XmlControl
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
    
}