using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;

namespace BoTech.AvaloniaDesigner.Services.Avalonia;

public static class TypeCastingService
{
    /// <summary>
    /// Checks if the given Control is a Layout Control ("Border", "Canvas", "DockPanel", "Expander", "Grid", "GridSplitter", "Panel", "RelativePanel", "ScrollViewer", "SplitView", "StackPanel", "TabControl", "UniformGrid", "WrapPanel") and has the <b>Childs</b> Property.
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    public static bool IsLayoutControl(Control control)
    {
        // Possible Layout Types
        string[] layoutControlNames = ["Border", "Canvas", "DockPanel", "Expander", "Grid", "GridSplitter", "Panel", "RelativePanel", "ScrollViewer", "SplitView", "StackPanel", "TabControl", "UniformGrid", "WrapPanel"];
        if (layoutControlNames.Contains(control.GetType().Name)) return true; //control.GetType().GetField("Childs") != null;
        return false;
    }
    /// <summary>
    /// Checks if the given ControlTypeName is a Layout Control ("Border", "Canvas", "DockPanel", "Expander", "Grid", "GridSplitter", "Panel", "RelativePanel", "ScrollViewer", "SplitView", "StackPanel", "TabControl", "UniformGrid", "WrapPanel") and has the <b>Childs</b> Property.
    /// </summary>
    /// <param name="controlTypeName"></param>
    /// <returns></returns>
    public static bool IsLayoutControl(string controlTypeName)
    {
        // Possible Layout Types
        string[] layoutControlNames = ["Border", "Canvas", "DockPanel", "Expander", "Grid", "GridSplitter", "Panel", "RelativePanel", "ScrollViewer", "SplitView", "StackPanel", "TabControl", "UniformGrid", "WrapPanel"];
        if (layoutControlNames.Contains(controlTypeName)) return true; //control.GetType().GetField("Childs") != null;
        return false;
    }
    
    /// <summary>
    /// This Method tries to get the Child, Children or Content property of the given Control and return it.
    /// </summary>
    /// <param name="control"></param>
    /// <returns>Can return null when the given Control is no LayoutControl.</returns>
    public static Controls? GetChildControlsOfLayoutControl(Control control)
    {
        if (TypeCastingService.IsLayoutControl(control))
        {
            // Try Different Names for the Same Attribute:
            PropertyInfo[] propertyInfos = control.GetType().GetProperties();
            PropertyInfo? info = propertyInfos.Where(p => p.Name == "Children").FirstOrDefault();
            
            if (info == null) info = propertyInfos.Where(p => p.Name == "Child").FirstOrDefault();
            if (info == null) info = propertyInfos.Where(p => p.Name == "Content").FirstOrDefault();
            
            if (info != null)
            {
                object? obj = info.GetValue(control);
                if (obj != null)
                {
                    return (Controls)obj;
                }
            }
        }
        return null;
    }

    public static List<TypeInfo> GetAllControlBasedAvaloniaTypes()
    {
        // Gets all Types which are nested under the Avalonia.? Namespace
        List<TypeInfo> allTypes = Assembly.Load(new AssemblyName("Avalonia.Controls")).DefinedTypes.ToList();
        // Therefore it is needed to get all Types which are directly nested under Avalonia.Controls
        List<TypeInfo> sortedTypes = allTypes.Where(type => type.Namespace == "Avalonia.Controls").ToList();
        
        // Because of in the new Sorted List are a lot of Types which we do not use for example Interfaces or Classes like Control, it is necessary to filter all Type which inherit from Control.
        return sortedTypes.Where(type => type.AsType().IsSubclassOf(typeof(Control))).ToList();
    }
}