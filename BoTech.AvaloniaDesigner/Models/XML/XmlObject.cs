using System;
using System.Collections.Generic;


namespace BoTech.AvaloniaDesigner.Models.XML;

public class XmlObject
{
    public required string NameOfType { get; set; }
    public List<XmlObject> Children { get; set; } = new List<XmlObject>();
    public required XmlObject Parent { get; set; }
    public List<XmlProperty> Properties { get; set; } = new List<XmlProperty>();
    public string DataBetween { get; set; } = string.Empty;
}