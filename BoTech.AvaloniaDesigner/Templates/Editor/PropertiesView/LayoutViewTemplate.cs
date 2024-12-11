using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using BoTech.AvaloniaDesigner.Services.PropertiesView;

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

    // Bindings for the Visual representation of the Margin settings of the Control
    
    private Subject<string> _bindingMarginTextTop = new();
    private Subject<string> _bindingMarginTextBottom = new();
    private Subject<string> _bindingMarginTextLeft = new();
    private Subject<string> _bindingMarginTextRight = new();
    
    // Bindings for the Visual representation of the PADDING settings of the Control

    private Subject<string> _bindingPaddingTextTop = new();
    private Subject<string> _bindingPaddingTextBottom = new();
    private Subject<string> _bindingPaddingTextLeft = new();
    private Subject<string> _bindingPaddingTextRight = new();
    public Control GetViewTemplateForControl(Control control)
    {
	    // Main Stackpanel
	    StackPanel stackPanel = new StackPanel();
	    // For the Visual Representation of the Margin and Padding:
	    TextBlock marginTextTop = new TextBlock()
	    {
		    HorizontalAlignment = HorizontalAlignment.Center,
		    Text = "5",
		    [!TextBlock.TextProperty] = _bindingMarginTextTop.ToBinding(),
	    };
	    DockPanel.SetDock(marginTextTop, Dock.Top);
	    
	    TextBlock marginTextBottom = new TextBlock()
	    {
		    HorizontalAlignment = HorizontalAlignment.Center,
		    Text = "10",
		    [!TextBlock.TextProperty] = _bindingMarginTextBottom.ToBinding()
	    };
	    DockPanel.SetDock(marginTextBottom, Dock.Bottom);
	    
	    TextBlock marginTextLeft = new TextBlock()
	    {
		    VerticalAlignment = VerticalAlignment.Center,
		    Text = "15",
		    [!TextBlock.TextProperty] = _bindingMarginTextLeft.ToBinding()
	    };
	    DockPanel.SetDock(marginTextLeft, Dock.Left);
	    
	    TextBlock marginTextRight = new TextBlock()
	    {
		    VerticalAlignment = VerticalAlignment.Center,
		    Text = "20",
		    [!TextBlock.TextProperty] = _bindingMarginTextRight.ToBinding()
	    };
	    DockPanel.SetDock(marginTextRight, Dock.Right);

	    
	    
	    TextBlock paddingTextTop = new TextBlock()
	    {
		    HorizontalAlignment = HorizontalAlignment.Center,
		    Text = "5",
		    [!TextBlock.TextProperty] = _bindingPaddingTextTop.ToBinding()
	    };
	    DockPanel.SetDock(paddingTextTop, Dock.Top);
	    
	    TextBlock paddingTextBottom = new TextBlock()
	    {
		    HorizontalAlignment = HorizontalAlignment.Center,
		    Text = "10",
		    [!TextBlock.TextProperty] = _bindingPaddingTextBottom.ToBinding()
	    };
	    DockPanel.SetDock(paddingTextBottom, Dock.Bottom);
	    
	    TextBlock paddingTextLeft = new TextBlock()
	    {
		    VerticalAlignment = VerticalAlignment.Center,
		    Text = "15",
		    [!TextBlock.TextProperty] = _bindingPaddingTextLeft.ToBinding()
	    };
	    DockPanel.SetDock(paddingTextLeft, Dock.Left);
	    
	    TextBlock paddingTextRight = new TextBlock()
	    {
		    VerticalAlignment = VerticalAlignment.Center,
		    Text = "20",
		    [!TextBlock.TextProperty] = _bindingPaddingTextRight.ToBinding()
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
				    borderPadding,
			    }
		    }
	    };
	    
	    stackPanel.Children.Add(borderMargin);
	    
	    // Creating the TextBoxes for the Margin

	    StandardViewTemplate standardViewTemplate = new StandardViewTemplate()
	    {
		    ReferencedProperties =
		    {
			    new StandardViewTemplate.ReferencedProperty("Margin", EditBoxOptions.Auto), // double example
			    new StandardViewTemplate.ReferencedProperty("HorizontalAlignment", EditBoxOptions.Auto),
			    new StandardViewTemplate.ReferencedProperty("VerticalAlignment", EditBoxOptions.Auto), // bool
			    new StandardViewTemplate.ReferencedProperty("ZIndex", EditBoxOptions.Auto), // Int example
			    new StandardViewTemplate.ReferencedProperty("Text", EditBoxOptions.Auto), // string
		    }
	    };
	    stackPanel.Children.Add(standardViewTemplate.GetViewTemplateForControl(control));
	    
	    
	    
	    //stackPanel.Children.Add(ControlsCreator.CreateEditBox(control, "IsVisible", EditBoxOptions.Auto));
	    
	    return stackPanel;
    }
}