using System;
using Avalonia.Controls;

namespace BoTech.AvaloniaDesigner.Templates.Editor.PropertiesView;

public interface IViewTemplate
{
     /// <summary>
     /// Will be displayed on the Expander Header
     /// </summary>
     public string Name { get; }
     public  Control GetViewTemplateForControl(Control control)
     {
          return new Control();
     }
  
}