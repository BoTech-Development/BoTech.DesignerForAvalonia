using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

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

    public  Control GetViewTemplateForControl(Control control)
    {
	    TextBlock marginTextTop = new TextBlock()
	    {
		    HorizontalAlignment = HorizontalAlignment.Center,
		    Text = "5"
	    };
	    DockPanel.SetDock(marginTextTop, Dock.Top);
	    
	    TextBlock marginTextBottom = new TextBlock()
	    {
		    HorizontalAlignment = HorizontalAlignment.Center,
		    Text = "10"
	    };
	    DockPanel.SetDock(marginTextBottom, Dock.Bottom);
	    
	    TextBlock marginTextLeft = new TextBlock()
	    {
		    VerticalAlignment = VerticalAlignment.Center,
		    Text = "15"
	    };
	    DockPanel.SetDock(marginTextLeft, Dock.Left);
	    
	    TextBlock marginTextRight = new TextBlock()
	    {
		    VerticalAlignment = VerticalAlignment.Center,
		    Text = "20"
	    };
	    DockPanel.SetDock(marginTextRight, Dock.Right);

	    
	    
	    TextBlock paddingTextTop = new TextBlock()
	    {
		    HorizontalAlignment = HorizontalAlignment.Center,
		    Text = "5"
	    };
	    DockPanel.SetDock(paddingTextTop, Dock.Top);
	    
	    TextBlock paddingTextBottom = new TextBlock()
	    {
		    HorizontalAlignment = HorizontalAlignment.Center,
		    Text = "10"
	    };
	    DockPanel.SetDock(paddingTextBottom, Dock.Bottom);
	    
	    TextBlock paddingTextLeft = new TextBlock()
	    {
		    VerticalAlignment = VerticalAlignment.Center,
		    Text = "15"
	    };
	    DockPanel.SetDock(paddingTextLeft, Dock.Left);
	    
	    TextBlock paddingTextRight = new TextBlock()
	    {
		    VerticalAlignment = VerticalAlignment.Center,
		    Text = "20"
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
	    return borderMargin;
    }
}