using System;
using System.Collections.Generic;
using Avalonia.Data;
using BoTech.DesignerForAvalonia.Models.Binding;
using BoTech.DesignerForAvalonia.Models.Project.CSharp;

namespace BoTech.DesignerForAvalonia.Services.Binding;

public class BindingManager
{
    // Text="{Binding Title, RelativeSource={RelativeSource Tree=Logical, Mode=FindAncestor, AncestorType=Window}, TargetNullValue={Binding MyInt}}" 
    public static ExtractedBindingInfo ParseBindingsFromSource(string propertySource)
    {
        Node root = new Node();
        CreateBindingNodeTree(root, propertySource);
        ExtractedBindingInfo? bindingInfo = ParseBindingFromNodeTree(root);
        return new ExtractedBindingInfo();
    }

    private static ExtractedBindingInfo? ParseBindingFromNodeTree(Node root)
    {
        if (root.Name.Equals("Binding"))
        {
            ExtractedBindingInfo bindingInfo = new ExtractedBindingInfo();
            if(root.FirstValue != string.Empty) bindingInfo.Path = root.FirstValue;
            foreach (KeyValuePair<string, object> attribute in root.Attributes)
            {
                switch (attribute.Key)
                {
                    case "Path":
                        if (attribute.Value is string path)
                        {
                            if (path.StartsWith("#"))
                            {
                                bindingInfo.Path = path.Substring(0, path.IndexOf('.'));
                                bindingInfo.ElementName = path.Substring(path.IndexOf('.') + 1);
                                break;
                            }
                            bindingInfo.Path = path; 
                            break;
                        }
                        throw new ArgumentException("The Binding Path must be a string.");
                    case "Mode":
                        try
                        {
                            if (attribute.Value is string mode)
                                bindingInfo.Mode = (BindingMode)Enum.Parse(typeof(BindingMode), attribute.Value.ToString());
                            else if (attribute.Value is Node node)
                                bindingInfo.Mode = ParseBindingFromNodeTree(node);
                        }catch(Exception e)
                        {
                            throw new ArgumentException("The Binding Mode must be a valid enum value. Detailed Error: " + e.Message);;
                        }
                        break;
                    case "ElementName":
                        if (bindingInfo.ElementName == string.Empty)
                        {
                            if (attribute.Value is string elementName)
                                bindingInfo.ElementName = elementName;
                            else if (attribute.Value is Node node)
                                bindingInfo.ElementName = ParseBindingFromNodeTree(node);
                            break;
                        }
                        else
                        {
                            throw new ArgumentException("The Binding ElementName can not be set twice: ElementName is already defined as: " + bindingInfo.ElementName + " and you try to set it to: " + attribute.Value.ToString() + " too!");
                        }
                    case "FallbackValue":
                        if (attribute.Value is string fallbackValue)
                            bindingInfo.FallbackValue = fallbackValue;
                        else if (attribute.Value is Node node)
                            bindingInfo.FallbackValue = ParseBindingFromNodeTree(node);
                        break;
                    case "TargetNullValue":
                        if (attribute.Value is string targetNullValue)
                            bindingInfo.TargetNullValue = targetNullValue;
                        else if (attribute.Value is Node node)
                            bindingInfo.TargetNullValue = ParseBindingFromNodeTree(node);
                        break;
                    case "UpdateSourceTrigger":
                        if (attribute.Value is string updateSourceTrigger)
                            bindingInfo.UpdateSourceTrigger = updateSourceTrigger;
                        else if (attribute.Value is Node node)
                            bindingInfo.UpdateSourceTrigger = ParseBindingFromNodeTree(node);
                        break;
                    case "RelativeSource":
                        if (attribute.Value is Node rnode) bindingInfo.RelativeSource = ParseRelativeSourceFromNodeTree(rnode);
                        break;
                }
            }
            return bindingInfo;
        }
        else
        {
            throw new ArgumentException("Parent Node must be from type Binding.");
        }

        return null;
    }

    private static ExtractedRelativeSource? ParseRelativeSourceFromNodeTree(Node root)
    {
        if (root.Name.Equals("RelativeSource"))
        {
            ExtractedRelativeSource relativeSource = new ExtractedRelativeSource();
            foreach (KeyValuePair<string, object> attribute in root.Attributes)
            {
                switch (attribute.Key)
                {
                    case "AncestorLevel":
                        if (attribute.Value is int ancestorLevel)
                            relativeSource.AncestorLevel = ancestorLevel;
                        else if (attribute.Value is Node node)
                            relativeSource.AncestorLevel = ParseBindingFromNodeTree(node);
                        break;
                    case "AncestorType":
                        if (attribute.Value is string ancestorType)
                            relativeSource.AncestorType = ancestorType;
                        else if (attribute.Value is Node node)
                            relativeSource.AncestorType = ParseBindingFromNodeTree(node);
                        break;
                    case "Mode":
                        if (Enum.IsDefined(typeof(RelativeSourceMode), attribute.Value))
                            relativeSource.Mode = Enum.Parse(typeof(RelativeSourceMode), attribute.Value.ToString()!);
                        else if (attribute.Value is Node node)
                            relativeSource.Mode = ParseBindingFromNodeTree(node);
                        break;
                    case "Tree":
                        if (Enum.IsDefined(typeof(TreeType), attribute.Value))
                            relativeSource.Tree = Enum.Parse(typeof(TreeType), attribute.Value.ToString()!);
                        else if (attribute.Value is Node node)
                            relativeSource.Tree = ParseBindingFromNodeTree(node);
                        break;
                }
            }
            return relativeSource;
        }
        else
        {
            throw new ArgumentException("Parent Node must be from type RelativeSource.");
        }

        return null;
    }
    /// <summary>
    /// Creates a Node tree recursively. The Node tree represents the Binding Structure.
    /// </summary>
    /// <param name="root">The Root node</param>
    /// <param name="propertySource">The string which contains the Binding.</param>
    /// <returns>An internal value which does not need to be evaluated after invoking this method.</returns>
    private static int CreateBindingNodeTree(Node root, string propertySource)
    {
        bool loadingName = true;
        bool loadingFirstValue = false;
        int nameIndex = 0;
        
        int lastIndex = 0;
        for (int c = 0; c < propertySource.Length; c++)
        {
            // Loading the Name => {Binding Text} => Name will be Binding
            if (loadingName && propertySource[c] == ' ')
            {
                root.Name += propertySource.Substring(0, c);
                nameIndex = c + 1;
                loadingName = false;
                loadingFirstValue = true;
            }
            // Loading the first value => {Binding Text} => FirstValue will be Text
            if (loadingFirstValue && (propertySource[c] == '}' || propertySource[c] == ','))
            {
                root.FirstValue += propertySource.Substring(nameIndex, c - nameIndex);
                loadingFirstValue = false;
                loadingName = false;
            }
            // There is no first Value, but the first Attribute => {Binding Mode=OneWay} => FirstAttribute is Mode=OneWay
            if (loadingFirstValue && propertySource[c] == '=' && propertySource[c + 1] != '{')
            {
                root.Attributes.Add(GetNameOfAttribute(c, propertySource), GetValueOfAttribute(c, propertySource));
                c++; // A further if decision would do the same if c were not incremented by one
                loadingFirstValue = false;
                loadingName = false;
            }
            // There is no first Value, but the first Attribute with a sub node => {Binding RelativeSource={RelativeSource Tree=Logical}} => FirstAttribute is RelativeSource={RelativeSource Tree=Logical}
            if (loadingFirstValue && propertySource[c] == '=' && propertySource[c + 1] == '{')
            {
                c++;
                
                root.Content += propertySource.Substring(lastIndex, c - lastIndex);
                
                Node child = new Node();
                child.Parent = root;
                // root.Children.Add(child);
                
                // Save in the Attribute Dictionary
                root.Attributes.Add(GetNameOfAttribute(c - 1, propertySource), child);
                
                // Parsing the subnode
                c += CreateBindingNodeTree(child, propertySource.Substring(c + 1)) + 1;
                lastIndex = c;
                if (c + 1 < propertySource.Length){ c++; lastIndex++;}
                loadingFirstValue = false;
                loadingName = false;
            }
            if (propertySource[c] == '}')
            {
                root.Content += propertySource.Substring(lastIndex, c - lastIndex);
                return c;
            }
            // New Attribute with a sub node as a value
            if (!loadingName && !loadingFirstValue && c + 1 < propertySource.Length && propertySource[c] == '=' && propertySource[c + 1] == '{')
            {
                c++;
                
                root.Content += propertySource.Substring(lastIndex, c - lastIndex);
                
                Node child = new Node();
                child.Parent = root;
               // root.Children.Add(child);
                
                // Save in the Attribute Dictionary
                root.Attributes.Add(GetNameOfAttribute(c - 1, propertySource), child);
                
                // Parsing the subnode
                c += CreateBindingNodeTree(child, propertySource.Substring(c + 1)) + 1;
                lastIndex = c;
                if (c + 1 < propertySource.Length){ c++; lastIndex++;}
            }
            // New Attribute with simple string as value
            if (!loadingName && !loadingFirstValue && c + 1 < propertySource.Length && propertySource[c] == '=' && propertySource[c + 1] != '{')
            {
                root.Attributes.Add(GetNameOfAttribute(c, propertySource), GetValueOfAttribute(c, propertySource));
            }
        }
        return propertySource.Length;
    }
    /// <summary>
    /// Searches for the name of the attribute => 'MyAttribute=MyValue' as propertySource => will return MyAttribute
    /// </summary>
    /// <param name="location">The string position where the attribute is located.</param>
    /// <param name="propertySource">The string which contains the Binding</param>
    /// <returns>The name of the Attribute</returns>
    private static string GetNameOfAttribute(int location, string propertySource)
    {
        for (int c = location; c > 0; c--)
        {
            if(propertySource[c] == ' ' || propertySource[c] == ',' || propertySource[c] == '}' || propertySource[c] == '{')
            {
                return propertySource.Substring(c + 1, location - c -1);
            }
        }
        return string.Empty;
    }
    /// <summary>
    /// Searches for the value of the attribute => 'MyAttribute=MyValue' as propertySource => will return MyValue
    /// </summary>
    /// <param name="location">The string position where the attribute is located.</param>
    /// <param name="propertySource">The string which contains the Binding</param>
    /// <returns>The value of the Attribute</returns>
    private static string GetValueOfAttribute(int location, string propertySource)
    {
        for (int c = location; c < propertySource.Length; c++)
        {
            if(propertySource[c] == ' ' || propertySource[c] == ',' || propertySource[c] == '}' || propertySource[c] == '{')
            {
                return propertySource.Substring(location, c - location).Replace("=", "").Replace(",", "");
            }
        }
        return string.Empty;
    }
    private class Node
    {
        /// <summary>
        /// The name of the Node => {Binding Text} => Name will be Binding
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// The first value of the Node => {Binding Text} => FirstValue will be Text
        /// </summary>
        public string FirstValue { get; set; } = string.Empty;
        /// <summary>
        /// The plain content
        /// </summary>
        public string Content { get; set; } = string.Empty;
        /// <summary>
        /// The parent node.
        /// </summary>
        public Node? Parent { get; set; }
        //public List<Node> Children { get; set; } = new List<Node>();
        /// <summary>
        /// All Attributes that are defined => could be new Node when it is a complex Attribute. Otherwise, it is a simple string.
        /// </summary>
        public Dictionary<string, object> Attributes { get; set; } = new Dictionary<string, object>();
    }
    
}