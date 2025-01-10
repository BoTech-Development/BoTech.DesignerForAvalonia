using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using BoTech.AvaloniaDesigner.Controller.Editor;
using BoTech.AvaloniaDesigner.Models.XML;
using BoTech.AvaloniaDesigner.Services.PropertiesView;
using BoTech.AvaloniaDesigner.ViewModels.Editor;

namespace BoTech.AvaloniaDesigner.Templates.Editor.PropertiesView;

public class LayoutViewTemplate : IViewTemplate
{
	
	
    /*
     * <Border BorderBrush="Blue" BorderThickness="2" Padding="5" Margin="10" Background="LightBlue">
    	<DockPanel>
    		<TextBlock DockPanel.Dock="Top" Text="5" HorizontalAlignment="Center"/>
    		<TextBlock DockPanel.Dock="Right" Text="10" VerticalAlignment="Center"/>
    		<TextBlock DockPanel.Dock="Bottom" Text="15" HorizontalAlignment="Center"/>
    		<TextBlock DockPanel.Dock="Left" Text="20" VerticalAlignment="Center"/>
  		  <Border BorderBrush="Green" BorderThickness="2" Padding="5" Margin="10" Background="LightGreen">
    			<DockPanel>
    				<TextBlock DockPanel.Dock="Top" Text="5" HorizontalAlignment="Center"/>
    				<TextBlock DockPanel.Dock="Right" Text="10" VerticalAlignment="Center"/>
    				<TextBlock DockPanel.Dock="Bottom" Text="15" HorizontalAlignment="Center"/>
    				<TextBlock DockPanel.Dock="Left" Text="20" VerticalAlignment="Center"/>
  			
    			</DockPanel>
    		</Border>
    	</DockPanel>
    </Border>
     */
    public string Name { get; } = "Basic Layout";
    public List<StandardViewTemplate> StandardViewTemplates { get; set;  }

	// Vars for the Visual representation of the Margin Property
	private double _marginTop;
	private double _marginLeft;
	private double _marginRight;
	private double _marginBottom;
	
	// Vars for the Visual representation of the Border Property
	private double _borderTop;
	private double _borderLeft;
	private double _borderRight;
	private double _borderBottom;
	
	// Vars for the Visual representation of the Padding Property
	private double _paddingTop;
	private double _paddingLeft;
	private double _paddingRight;
	private double _paddingBottom;
    

    public Control GetViewTemplateForControl(XmlControl xmlControl, PropertiesViewModel.TabContent tabContent)
    {
	    // Main Stackpanel
	    StackPanel stackPanel = new StackPanel();
	    
	    
	    stackPanel.Children.Add(CreateCustomMarginAndPaddingView(xmlControl));
	    
	    // Creating the TextBoxes for the Margin
		
	    StandardViewTemplates = new List<StandardViewTemplate>();
	    StandardViewTemplate stdViewTemplate = new StandardViewTemplate()
	    {
		    ReferencedProperties =
		    {
			    new StandardViewTemplate.ReferencedProperty("HorizontalAlignment", xmlControl.Control, EditBoxOptions.Auto),
			    new StandardViewTemplate.ReferencedProperty("VerticalAlignment", xmlControl.Control, EditBoxOptions.Auto), 
			    new StandardViewTemplate.ReferencedProperty("HorizontalContentAlignment", xmlControl.Control, EditBoxOptions.Auto),
			    new StandardViewTemplate.ReferencedProperty("VerticalContentAlignment", xmlControl.Control, EditBoxOptions.Auto),
			    new StandardViewTemplate.ReferencedProperty("ZIndex", xmlControl.Control, EditBoxOptions.Auto), 
			    new StandardViewTemplate.ReferencedProperty("Bounds", xmlControl.Control, EditBoxOptions.Auto),
			    new StandardViewTemplate.ReferencedProperty("DesiredSize", xmlControl.Control, EditBoxOptions.Auto),
			    new StandardViewTemplate.ReferencedProperty("Width", xmlControl.Control, EditBoxOptions.Auto),
			    new StandardViewTemplate.ReferencedProperty("Height", xmlControl.Control, EditBoxOptions.Auto),
			    new StandardViewTemplate.ReferencedProperty("MaxHeight", xmlControl.Control, EditBoxOptions.Auto),
			    new StandardViewTemplate.ReferencedProperty("MaxWidth", xmlControl.Control, EditBoxOptions.Auto),
			    new StandardViewTemplate.ReferencedProperty("MinHeight", xmlControl.Control, EditBoxOptions.Auto),
			    new StandardViewTemplate.ReferencedProperty("MinWidth", xmlControl.Control, EditBoxOptions.Auto),
		    }
	    };
	    // Saving the Standard View Template for the event handling
	    StandardViewTemplates.Add(stdViewTemplate);
	    stackPanel.Children.Add(stdViewTemplate.GetViewTemplateForControl(xmlControl, tabContent, Name));
	    return stackPanel;
    }

    public Control GetRerenderedViewTemplateForControl(XmlControl xmlControl, PropertiesViewModel.TabContent tabContent)
    {
	    StackPanel stackPanel = new StackPanel();
	    // It is necessary because otherwise the Custom Margin Control will not appear on the Properties Editor.
	    stackPanel.Children.Add(CreateCustomMarginAndPaddingView(xmlControl));
	    foreach (StandardViewTemplate standardViewTemplate in StandardViewTemplates)
	    {
		    stackPanel.Children.Add(standardViewTemplate.GetViewTemplateForControl(xmlControl, tabContent, Name));
	    }
	    return stackPanel;
    }

    private Border CreateCustomMarginAndPaddingView(XmlControl xmlControl)
    {
	    PropertyInfo? marginPropertyInfo = xmlControl.Control.GetType().GetProperty("Margin");
	    
	    // Default Values for margin:
	    Thickness? margin = marginPropertyInfo?.GetValue(xmlControl.Control) as Thickness?;
	    if (margin != null)
	    {
		    _marginTop = margin.Value.Top;
		    _marginLeft = margin.Value.Left;
		    _marginRight = margin.Value.Right;
		    _marginBottom = margin.Value.Bottom;
	    }
	    
	    PropertyInfo? paddingPropertyInfo = xmlControl.Control.GetType().GetProperty("Padding");
	    
	    // Default Values for padding:
	    Thickness? padding = paddingPropertyInfo?.GetValue(xmlControl.Control) as Thickness?;
	    if (padding != null)
	    {
			_paddingTop = padding.Value.Top;
			_paddingLeft = padding.Value.Left;
			_paddingRight = padding.Value.Right;
			_paddingBottom = padding.Value.Bottom;
	    }
	    
	    PropertyInfo? borderThicknessProperty = xmlControl.Control.GetType().GetProperty("BorderThickness");
	    
	    //Default Values for border Thickness
	    Thickness? borderThickness = borderThicknessProperty?.GetValue(xmlControl.Control) as Thickness?;
	    if (borderThickness != null)
	    {
		    _borderTop = borderThickness.Value.Top;
		    _borderLeft = borderThickness.Value.Left;
		    _borderRight = borderThickness.Value.Right;
		    _borderBottom = borderThickness.Value.Bottom;
	    }
	    
	    // For the Visual Representation of the Margin and Padding:
	    TextBox marginTextTop = new TextBox()
	    {
		    HorizontalAlignment = HorizontalAlignment.Center,
		    Text = _marginTop.ToString(),
	    };
	    marginTextTop.TextChanged += (s, e) =>
	    {
		    try
		    {
			    _marginTop = double.Parse(marginTextTop.Text);
			    if (marginPropertyInfo != null)
				    EditorController.OnPropertyInPropertiesViewChanged(xmlControl, marginPropertyInfo,
					    new Thickness(_marginLeft, _marginTop, _marginRight, _marginBottom));
		    }
		    catch (Exception ex)
		    {
			    Console.WriteLine(ex.Message);
		    }
	    };
	    DockPanel.SetDock(marginTextTop, Dock.Top);
	    
	    TextBox marginTextBottom = new TextBox()
	    {
		    HorizontalAlignment = HorizontalAlignment.Center,
		    Text = _marginBottom.ToString(),
	    };
	    marginTextBottom.TextChanged += (s, e) =>
	    {
		    try
		    {
			    _marginBottom = double.Parse(marginTextBottom.Text);
			    if (marginPropertyInfo != null)
				    EditorController.OnPropertyInPropertiesViewChanged(xmlControl, marginPropertyInfo,
					    new Thickness(_marginLeft, _marginTop, _marginRight, _marginBottom));
		    }
		    catch (Exception ex)
		    {
			    Console.WriteLine(ex.Message);
		    }
	    };
	    DockPanel.SetDock(marginTextBottom, Dock.Bottom);
	    
	    TextBox marginTextLeft = new TextBox()
	    {
		    VerticalAlignment = VerticalAlignment.Center,
		    Text = _marginLeft.ToString(),
	    };
	    marginTextLeft.TextChanged += (s, e) =>
	    {
		    try
		    {
			    _marginLeft = double.Parse(marginTextLeft.Text);
			    if (marginPropertyInfo != null)
				    EditorController.OnPropertyInPropertiesViewChanged(xmlControl, marginPropertyInfo,
					    new Thickness(_marginLeft, _marginTop, _marginRight, _marginBottom));
		    }
		    catch (Exception ex)
		    {
			    Console.WriteLine(ex.Message);
		    }
	    };
	    DockPanel.SetDock(marginTextLeft, Dock.Left);

	    TextBox marginTextRight = new TextBox()
	    {
		    VerticalAlignment = VerticalAlignment.Center,
		    Text = _marginRight.ToString(),
	    };
	    marginTextRight.TextChanged += (s, e) =>
	    {
		    try
		    {
			    _marginRight= double.Parse(marginTextRight.Text);
			    if (marginPropertyInfo != null)
				    EditorController.OnPropertyInPropertiesViewChanged(xmlControl, marginPropertyInfo,
					    new Thickness(_marginLeft, _marginTop, _marginRight, _marginBottom));
		    }
		    catch (Exception ex)
		    {
			    Console.WriteLine(ex.Message);
		    }
	    };
	    DockPanel.SetDock(marginTextRight, Dock.Right);
	    
	    
		// Border:
		
		
		 TextBox borderTextTop = new TextBox()
	    {
		    HorizontalAlignment = HorizontalAlignment.Center,
		    Text = _borderTop.ToString(),
	    };
	    borderTextTop.TextChanged += (s, e) =>
	    {
		    try
		    {
			    _borderTop = double.Parse(borderTextTop.Text);
			    if (borderThicknessProperty != null)
				    EditorController.OnPropertyInPropertiesViewChanged(xmlControl, borderThicknessProperty,
					    new Thickness(_borderLeft, _borderTop, _borderRight, _borderBottom));
		    }
		    catch (Exception ex)
		    {
			    Console.WriteLine(ex.Message);
		    }
	    };
	    DockPanel.SetDock(borderTextTop, Dock.Top);
	    
	    TextBox borderTextBottom = new TextBox()
	    {
		    HorizontalAlignment = HorizontalAlignment.Center,
		    Text = _borderBottom.ToString(),
	    };
	    borderTextBottom.TextChanged += (s, e) =>
	    {
		    try
		    {
			    _borderBottom = double.Parse(borderTextBottom.Text);
			    if (borderThicknessProperty != null)
				    EditorController.OnPropertyInPropertiesViewChanged(xmlControl, borderThicknessProperty,
					    new Thickness(_marginLeft, _marginTop, _marginRight, _marginBottom));
		    }
		    catch (Exception ex)
		    {
			    Console.WriteLine(ex.Message);
		    }
	    };
	    DockPanel.SetDock(borderTextBottom, Dock.Bottom);
	    
	    TextBox borderTextLeft = new TextBox()
	    {
		    VerticalAlignment = VerticalAlignment.Center,
		    Text = _borderLeft.ToString(),
	    };
	    borderTextLeft.TextChanged += (s, e) =>
	    {
		    try
		    {
			    _borderLeft = double.Parse(borderTextLeft.Text);
			    if (borderThicknessProperty != null)
				    EditorController.OnPropertyInPropertiesViewChanged(xmlControl, borderThicknessProperty,
					    new Thickness(_borderLeft, _borderTop, _borderRight, _borderBottom));
		    }
		    catch (Exception ex)
		    {
			    Console.WriteLine(ex.Message);
		    }
	    };
	    DockPanel.SetDock(borderTextLeft, Dock.Left);

	    TextBox borderTextRight = new TextBox()
	    {
		    VerticalAlignment = VerticalAlignment.Center,
		    Text = _borderRight.ToString(),
	    };
	    borderTextRight.TextChanged += (s, e) =>
	    {
		    try
		    {
			    _borderRight= double.Parse(borderTextRight.Text);
			    if (borderThicknessProperty != null)
				    EditorController.OnPropertyInPropertiesViewChanged(xmlControl, borderThicknessProperty,
					    new Thickness(_borderLeft, _borderTop, _borderRight, _borderBottom));
		    }
		    catch (Exception ex)
		    {
			    Console.WriteLine(ex.Message);
		    }
	    };
	    DockPanel.SetDock(borderTextRight, Dock.Right);

	    // Padding
	    
	    TextBox paddingTextTop = new TextBox()
	    {
		    HorizontalAlignment = HorizontalAlignment.Center,
		    Text = _paddingTop.ToString(),
	    };
	    paddingTextTop.TextChanged += (s, e) =>
	    {
		    try
		    {
			    _paddingTop = double.Parse(paddingTextTop.Text);
			    if (paddingPropertyInfo != null)
				    EditorController.OnPropertyInPropertiesViewChanged(xmlControl, paddingPropertyInfo,
					    new Thickness(_paddingLeft, _paddingTop, _paddingRight, _paddingBottom));
		    }
		    catch (Exception ex)
		    {
			    Console.WriteLine(ex.Message);
		    }
	    };
	    DockPanel.SetDock(paddingTextTop, Dock.Top);
	    
	    TextBox paddingTextBottom = new TextBox()
	    {
		    HorizontalAlignment = HorizontalAlignment.Center,
		    Text = _paddingBottom.ToString(),
	    };
	    paddingTextBottom.TextChanged += (s, e) =>
	    {
		    try
		    {
			    _paddingBottom = double.Parse(paddingTextBottom.Text);
			    if (paddingPropertyInfo != null)
				    EditorController.OnPropertyInPropertiesViewChanged(xmlControl, paddingPropertyInfo,
					    new Thickness(_paddingLeft, _paddingTop, _paddingRight, _paddingBottom));
		    }
		    catch (Exception ex)
		    {
			    Console.WriteLine(ex.Message);
		    }
	    };
	    DockPanel.SetDock(paddingTextBottom, Dock.Bottom);
	    
	    TextBox paddingTextLeft = new TextBox()
	    {
		    VerticalAlignment = VerticalAlignment.Center,
		    Text = _paddingLeft.ToString(),
	    };
	    paddingTextLeft.TextChanged += (s, e) =>
	    {
		    try
		    {
			    _paddingLeft = double.Parse(paddingTextLeft.Text);
			    if (paddingPropertyInfo != null)
				    EditorController.OnPropertyInPropertiesViewChanged(xmlControl, paddingPropertyInfo,
					    new Thickness(_paddingLeft, _paddingTop, _paddingRight, _paddingBottom));
		    }
		    catch (Exception ex)
		    {
			    Console.WriteLine(ex.Message);
		    }
	    };
	    DockPanel.SetDock(paddingTextLeft, Dock.Left);
	    
	    TextBox paddingTextRight = new TextBox()
	    {
		    VerticalAlignment = VerticalAlignment.Center,
		    Text = _paddingRight.ToString(),
	    };
	    paddingTextRight.TextChanged += (s, e) =>
	    {
		    try
		    {
			    _paddingRight = double.Parse(paddingTextRight.Text);
			    if (paddingPropertyInfo != null)
				    EditorController.OnPropertyInPropertiesViewChanged(xmlControl, paddingPropertyInfo,
					    new Thickness(_paddingLeft, _paddingTop, _paddingRight, _paddingBottom));
		    }
		    catch (Exception ex)
		    {
			    Console.WriteLine(ex.Message);
		    }
	    };
	    DockPanel.SetDock(paddingTextRight, Dock.Right);
	    
	    
	    Border borderPadding = new Border()
	    {
		    BorderBrush = Brushes.Green,
		    BorderThickness = new Thickness(2),
		    Margin = new Thickness(10),
		    Padding = new Thickness(5),
		    Background = Brushes.PaleGreen,
		    Child = new DockPanel()
		    {
			    Children =
			    {
				    paddingTextTop,
				    paddingTextBottom,
				    paddingTextLeft,
				    paddingTextRight,
				    new Border()
				    {
					    BorderBrush = Brushes.Gray,
					    Background = Brushes.Gray,
					    BorderThickness = new Thickness(2),
					    Margin = new Thickness(10),
					    Padding = new Thickness(5)
				    }
			    }
		    }
	    };
	    
	    Border borderBorderThickness = new Border()
	    {
		    BorderBrush = Brushes.Orange,
		    BorderThickness = new Thickness(2),
		    Margin = new Thickness(10),
		    Padding = new Thickness(5),
		    Background = Brushes.Yellow,
		    Child = new DockPanel()
		    {
			    Children =
			    {
					borderTextTop,
					borderTextBottom,
					borderTextLeft,
					borderTextRight,
					borderPadding,
			    }
		    }
	    };
	    
	    
	    Border borderMargin = new Border()
	    {
		    BorderBrush = Brushes.Blue,
		    BorderThickness = new Thickness(2),
		    Margin = new Thickness(10),
		    Padding = new Thickness(5),
		    Background = Brushes.LightSkyBlue,
		    Child = new DockPanel()
		    {
			    Children =
			    {
				    marginTextTop,
				    marginTextBottom,
				    marginTextLeft,
				    marginTextRight,
				    borderBorderThickness,
			    }
		    }
	    };
	    return borderMargin;
    }
}