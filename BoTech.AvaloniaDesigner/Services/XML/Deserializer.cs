using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using BoTech.AvaloniaDesigner.Models.XML;

namespace BoTech.AvaloniaDesigner.Services.XML;

public class Deserializer
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
    
    private List<TypeInfo> AllControlTypes { get; set; }
    public Deserializer()
    {
        AllControlTypes = Assembly.Load(new AssemblyName("Avalonia.Controls")).DefinedTypes.ToList();
    }
    /*
     * 
     <SP>
	    <TB Text="Hellord"/>
	    <BTN>Click Me</BTN>
	    <SP Orientation="Horizontal">
		    <TB>Status:...</TB>	
		    <BTN>Reset</BTN>
	    </SP>
    </SP>
     */
    public Control Deserialize(string xml, Assembly assembly)
    {
        XmlObject parent = new XmlObject()
        {
            NameOfType = "Parent",
            Parent = null
        };
        //ConvertXmlIntoNodes("<SP><TB Text=\"Hellord\"/><BTN>Click Me</BTN><SP Orientation=\"Horizontal\"><TB>Status:...</TB><BTN>Reset</BTN></SP></SP>", parent);
        ConvertXmlIntoNodes("<StackPanel><TextBlock Text=\"Hellord\"/><Button>Click Me</Button><StackPanel><TextBlock>Status:...</TextBlock><Button>Reset</Button></StackPanel></StackPanel>", parent);
        ConvertXmlPropertiesInXmlBindings(parent);
        Control control = TranslateToControls(parent.Children[0]);
        return control;
    }

    private Control TranslateToControls(XmlObject current)
    {
        Control control;
        TypeInfo? type = null;
        if ((type = AllControlTypes.Find(t => t.Name == current.NameOfType)) != null)
        {
            control = (Control)Activator.CreateInstance(type);
        }
        else
        {
            control = new TextBlock()
            {
                Text = "Can not find: " + current.NameOfType + " int the Avalonia.Controls dll."
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
            if (current.Children.Count >= 1)
            {
                List<Control> children = new List<Control>();
                foreach (XmlObject child in current.Children)
                {
                    children.Add(TranslateToControls(child));
                }
                if (propertyInfo.GetValue(control) is Controls childrenList)
                {
                    childrenList.AddRange(children);
                }
            }
            else if (current.DataBetween != string.Empty)
            {
                propertyInfo.SetValue(control, new TextBlock()
                {
                    Text = current.DataBetween,
                });
            }
        }

        if ((propertyInfo = control.GetType().GetProperty("Content")) != null)
        {
            SetChildOrTextAsContent(propertyInfo, current, control);
        }

        if ((propertyInfo = control.GetType().GetProperty("Text")) != null)
        {
            if (current.DataBetween != string.Empty)
            {
                propertyInfo.SetValue(control, current.DataBetween);
            }
        }
        
        return control;
    }

    private void SetChildOrTextAsContent(PropertyInfo propertyInfo, XmlObject current, Control control)
    {
        if (current.Children.Count == 1)
        {
            propertyInfo.SetValue(control, TranslateToControls(current.Children[0]));
        }
        else if(current.DataBetween != string.Empty)
        {
            propertyInfo.SetValue(control, new TextBlock()
            {
                Text = current.DataBetween,
            });
        }
    }
    /// <summary>
    /// Adds all Properties but no Bindings to the Control, that the Parser has read before.
    /// </summary>
    /// <param name="current"></param>
    /// <param name="control"></param>
    private void AddPropertiesToControl(XmlObject current, Control control)
    {
        foreach (XmlProperty property in current.Properties)
        {
            PropertyInfo? propertyInfo = null;
            // Check if Property is available
            if ((propertyInfo = control.GetType().GetProperty(property.PropertyName)) != null)
            {
                if(propertyInfo.CanWrite) propertyInfo.SetValue(control, Convert.ChangeType(property.PropertyValue, propertyInfo.PropertyType));
            }
        }
    }
    private void ConvertXmlPropertiesInXmlBindings(XmlObject parent)
    {
        foreach (XmlProperty property in parent.Properties)
        {
            if (property.PropertyValue.StartsWith(OpenBindingTag) && property.PropertyValue.EndsWith(CloseBindingTag))
            {
                string valueWithoutBindingTags = property.PropertyValue.Replace("{", "").Replace("}", "");
                string[] seperated = valueWithoutBindingTags.Split(" ");
                if (seperated.Length == 2)
                {
                    property.Binding = new XmlBinding()
                    {
                        BindingType = seperated[0],
                        BindingValue = seperated[1],
                    };
                }
            }
        }

        foreach (XmlObject child in parent.Children)
        {
            ConvertXmlPropertiesInXmlBindings(child);
        }
    }
    private void ConvertXmlIntoNodes(string xml, XmlObject parentNode, int currentIndex = 0)
    {
        XmlObject currentNode;
        // Search for open "<" Tag
        while (xml[currentIndex] != OpenTag)
        {
            if(currentIndex == xml.Length - 1) return;
            // Do nothing
            currentIndex++;
        }

        currentIndex++;
        
        // This variable is needed to avoid that XML nodes like </StackPanel> are not evaluated and processed as a new node.
        // This would result in a new XmlObject being instantiated for each XML node.
        int checkCount = 0;
        
        // Load the Type Name
        string typeName = string.Empty;
        // Ends if the String is at the end or the current Character is "/", ">" or " "
        while ((xml[currentIndex] != Spacer && xml[currentIndex] != CloseTag && xml[currentIndex] != EndTag))
        {
            if(currentIndex == xml.Length - 1) return;
            typeName += xml[currentIndex];
            currentIndex++;
            checkCount++;
        }

        if (checkCount >= 2)
        {
            currentNode = new XmlObject()
            {
                Parent = parentNode,
                NameOfType = typeName,
            };
            parentNode.Children.Add(currentNode);

            switch (xml[currentIndex])
            {
                // A Property will follow
                case Spacer:
                    LoadParameters(xml, currentIndex, currentNode, parentNode);
                    break;
                // There will be Data or Xml between
                case CloseTag:
                    LoadDataBetween(xml, currentIndex, currentNode, parentNode);
                    break;
                // The Xml Node ends, but there may be other nodes after this Node
                case EndTag:
                    // Load all other Nodes or Node
                    ConvertXmlIntoNodes(xml, parentNode, currentIndex);
                    break;
            }
        }

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="xml"></param>
    /// <param name="currentIndex"></param>
    /// <param name="currentXmlObject"></param>
    /// <param name="parentNode">Is needed because after this Xml-Node can be other Xml-Nodes.</param>
    private void LoadDataBetween(string xml, int currentIndex, XmlObject currentXmlObject, XmlObject parentNode)
    {
        currentIndex++;
        if (xml[currentIndex] == OpenTag)
        {
            // There is another xml Node embedded in this Node
            ConvertXmlIntoNodes(xml, currentXmlObject, currentIndex);
        }
        else
        {
            // Load Data
            string data = string.Empty;
            while (xml[currentIndex] != OpenTag)
            {
                if(currentIndex == xml.Length - 1) return;
                data += xml[currentIndex];
                currentIndex++;
            }

            while (xml[currentIndex] != CloseTag)
            {
                if(currentIndex == xml.Length - 1) return;
                currentIndex++;
            }
            currentIndex++;
            currentXmlObject.DataBetween = data;
            ConvertXmlIntoNodes(xml, parentNode, currentIndex);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="xml"></param>
    /// <param name="currentIndex"></param>
    /// <param name="currentXmlObject"></param>
    /// <param name="parentNode">Parent is needed when the currentXmlObject is defined as : &#60;YourClass YourProperty=""/&#62;, because there may be other Xml-Nodes after this Node which has to be added to the Parent Node.</param>
    private void LoadParameters(string xml, int currentIndex, XmlObject currentXmlObject, XmlObject parentNode)
    {
        while (xml[currentIndex] == Spacer)
        {
            currentIndex++;
            string propertyName = string.Empty;
            while (xml[currentIndex] != PropertyTag)
            {
                if(currentIndex == xml.Length - 1) return;
                propertyName += xml[currentIndex];
                currentIndex++;
            }

            XmlProperty property = new XmlProperty()
            {
                PropertyName = propertyName,
            };
            // Waiting for first '"'
            while (xml[currentIndex] != OpenOrCloseValueTag)
            {
                if(currentIndex == xml.Length - 1) return;
                currentIndex++;
            }

            currentIndex++;
            string value = string.Empty;
            while (xml[currentIndex] != OpenOrCloseValueTag)
            {
                if(currentIndex == xml.Length - 1) return;
                value += xml[currentIndex];
                currentIndex++;
            }

            property.PropertyValue = value;
            currentXmlObject.Properties.Add(property);
            currentIndex++;
           // currentIndex++;
        }

        switch (xml[currentIndex])
        {
            case CloseTag:
                LoadDataBetween(xml, currentIndex, currentXmlObject, parentNode);
                break;
            // The Xml Node ends, but there may be other nodes after this Node
            case EndTag:
                // Load all other Nodes or Node
                ConvertXmlIntoNodes(xml, parentNode, currentIndex);
                break;
        }
    }

}