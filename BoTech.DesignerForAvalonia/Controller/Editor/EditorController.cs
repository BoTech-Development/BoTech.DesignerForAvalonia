using System;
using System.Linq;
using System.Reflection;
using System.Xml;
using Avalonia.Controls;
using Avalonia.Layout;
using BoTech.DesignerForAvalonia.Models.Editor;
using BoTech.DesignerForAvalonia.Models.XML;
using BoTech.DesignerForAvalonia.ViewModels;
using BoTech.DesignerForAvalonia.ViewModels.Editor;
using BoTech.DesignerForAvalonia.Views.Editor;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.Controller.Editor;

public class EditorController : ViewModelBase
{
 
    // All Views that are needed for the Editor:
    public SolutionExplorerView SolutionExplorerView { get; set; }
    public PreviewView PreviewView { get; set; }
    public ItemsExplorerView ItemsView { get; set; } 
    public ViewHierarchyView ViewHierarchyView { get; set; }
    public PropertiesView PropertiesView { get; set; }
    
    
    public ViewHierarchyViewModel? ViewHierarchyViewModel { get; set; }
    public PropertiesViewModel? PropertiesViewModel { get; set; }
    
    /// <summary>
    /// What the user want to do
    /// </summary>
    public EDragAndDropOperation Operation { get; set; }
    /// <summary>
    /// Selected Control which can be dropped to the Preview.
    /// </summary>
    public Control? CurrentControl { get; set; }
    /// <summary>
    /// The Selected Control which the User can Select from the Hierarchy View
    /// </summary>
    public Control? SelectedControl { get; set; }

    /// <summary>
    /// Stores the deserialized Xml Node with all ChildNode.
    /// It is needed to make the serialization process easier, and it is easier to hold the Comments in the xml Documents.
    /// </summary>
    public XmlControl RootConnectedNode { get; set; }
    
    private Control _previewContent = new Grid();

    /// <summary>
    /// Will be set by the <see cref="PreviewViewModel"/> when the user hase changed something.
    /// </summary>
    public Control PreviewContent
    {
        get => _previewContent; 
        set => this.RaiseAndSetIfChanged(ref _previewContent, value);
    }
    
    public string OpenedFilePath { get; set; }
    
   
    
    public EditorController()
    {
        Operation = EDragAndDropOperation.None;
        CurrentControl = null;
        
        // Init PreviewContent
        TextBlock pleaseWait = new TextBlock()
        {
            Text = "Please wait while initialisation...",
        };
        PreviewContent = new Grid()
        {
            Children = { pleaseWait }
        };

    }
    /// <summary>
    /// The Controller has to be Initialized before it can run.
    /// The Init Method is used to create all Views.
    /// <returns>A grid is returned that contains all views belonging to the editor.</returns>
    /// </summary>
    public StackPanel Init(string selectedPath, string projectName)
    {
        SolutionExplorerView = new SolutionExplorerView()
        {
            DataContext = new SolutionExplorerViewModel(projectName, selectedPath, this),
            //[Grid.ColumnProperty] = 0
        };
        PreviewView = new PreviewView()
        {
            DataContext = new PreviewViewModel(this)
            
        };
        ItemsView = new ItemsExplorerView()
        {
            DataContext = new ItemsExplorerViewModel(this)
        };
        ViewHierarchyView = new ViewHierarchyView()
        {
            DataContext = new ViewHierarchyViewModel(this)
        };
        PropertiesView = new PropertiesView()
        {
            DataContext = new PropertiesViewModel(this)
        };
        return new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            Children =
            {
                SolutionExplorerView,
                ItemsView,
                PreviewView,
                ViewHierarchyView,
                PropertiesView,
            },
           // ColumnDefinitions = new ColumnDefinitions("Auto, Auto, Auto, Auto ,Auto"),
        };   
    }

    
    /// <summary>
    /// This Method tries to apply the new Value to the referenced Property of the Control
    /// </summary>
    /// <param name="xmlControl"></param>
    /// <param name="propertyInfo"></param>
    /// <param name="newValue"></param>
    public static void OnPropertyInPropertiesViewChanged(XmlControl xmlControl, PropertyInfo propertyInfo, object? newValue)
    {
        try
        {
            propertyInfo.SetValue(xmlControl.Control, newValue);
            // Apply the new Value: 
            UpdatePropertyInXmlControl(xmlControl, propertyInfo, newValue);
        }
        catch (Exception e)
        {
            // When the Property could not be converted
            if (newValue != null)
            {
                if (newValue.GetType() != propertyInfo.PropertyType)
                {
                    try
                    {
                        // Typecasting when the Type of the new Value is not equals with the Requested Type of the Property
                        propertyInfo.SetValue(xmlControl.Control, Convert.ChangeType(newValue, propertyInfo.PropertyType));
                        // Apply the new Value: 
                        UpdatePropertyInXmlControl(xmlControl, propertyInfo, newValue);
                    }
                    catch (Exception e2)
                    {
                        Console.WriteLine($"Exception in OnPropertyInPropertiesViewChanged: {e2}");
                    }
                }
                else
                {
                    // Property might be readonly.
                    Console.WriteLine(e);
                }
            }
            else
            {
                // Property might be readonly.
                Console.WriteLine(e);
            }
        }
        
    }
    /// <summary>
    /// Updates or create a new XmlAttribute for the given Property and its new Value.
    /// </summary>
    /// <param name="xmlControl"></param>
    /// <param name="propertyInfo"></param>
    /// <param name="newValue"></param>
    private static void UpdatePropertyInXmlControl(XmlControl xmlControl, PropertyInfo propertyInfo, object? newValue)
    {
        if (newValue != null && xmlControl.Node.Attributes != null)
        {
            // When ist primitive the ToString() Method can be applied to edit or create a new Attribute for the XmlNode.
            if (CheckIfPropertyNeedsXmlAttribute(propertyInfo))
            {
                XmlAttribute? selectedAttribute = null;
                foreach (XmlAttribute attribute in xmlControl.Node.Attributes)
                {
                    if (attribute.Name == propertyInfo.Name) selectedAttribute = attribute;
                }

                if (selectedAttribute != null)
                {
                    selectedAttribute.Value = newValue.ToString();
                }
                else
                {
                    // Attribute is not available in the Node, so it is necessary to create a new one.
                    if (xmlControl.Node.OwnerDocument != null)
                    {
                        XmlAttribute newAttribute = xmlControl.Node.OwnerDocument.CreateAttribute(propertyInfo.Name);
                        newAttribute.Value = newValue.ToString();
                        xmlControl.Node.Attributes.Append(newAttribute);
                    }
                }
            }
            else
            {
                // Create a new Inner Element to set the Attribute:
                UpdateClassPropertyInXmlControl(xmlControl, propertyInfo, newValue);
            }
        }
    }

    private static void UpdateClassPropertyInXmlControl(XmlControl xmlControl, PropertyInfo propertyInfo, object newValue)
    {
        // Removing the Attribute which is referenced to the given Property. It might be set by a primitife Value.
        if(xmlControl.Node.Attributes != null)
            if (xmlControl.Node.Attributes[propertyInfo.Name] != null)
                xmlControl.Node.Attributes.Remove(xmlControl.Node.Attributes[propertyInfo.Name]);
        if (xmlControl.Node.OwnerDocument != null)
        {
            XmlNode newNode = xmlControl.Node.OwnerDocument.CreateElement(xmlControl.Node.Name + "." + propertyInfo.Name);
            AddNonPrimitivePropertyToXmlNode(newNode, newValue);
        }
    }
    /*
     * <Grid>
     *  <Grid.ColumnDefinitions>
     *      <ColumnDefinition>
     *          <ColumnDefinition.Width>
     *          20
     *          </ColumnDefinition.Width>
     *      </ColumnDefinition>
     *  </Grid.ColumnDefinitions>
     * </Grid>
     */
    /// <summary>
    /// This Method can be used to add Properties which are not Primitve.
    /// </summary>
    /// <param name="parent">The Parent Node. It must be the Node with this Name  "YourClass.YourProperty"</param>
    /// <param name="value">The Value of the Property which should be placed.</param>
    private static void AddNonPrimitivePropertyToXmlNode(XmlNode parent, object value)
    { 
        XmlDocument? document = parent.OwnerDocument;
        if (document != null)
        {
            XmlNode newNode = document.CreateElement(value.GetType().Name);
            parent.AppendChild(newNode);
            // Adding all Properties to the new Node 
            PropertyInfo[] properties = value.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    if (CheckIfPropertyNeedsXmlAttribute(property) && newNode.Attributes != null)
                    {
                        XmlAttribute newAttribute = document.CreateAttribute(property.Name);
                        newAttribute.Value = value.ToString();
                        newNode.Attributes.Append(newAttribute);
                    }
                    else
                    {
                        if (property.GetValue(value) != null)
                        {
                            // Creating the XmlNode for : <YourClass.YourProperty></YourClass.YourProperty>
                            XmlNode propertyNode = document.CreateElement(value.GetType().Name + "." + property.Name);
                            newNode.AppendChild(propertyNode);
                            AddNonPrimitivePropertyToXmlNode(propertyNode, property.GetValue(value));
                        }
                    }
                }
            }
        }
    }

    private static bool CheckIfPropertyNeedsXmlAttribute(PropertyInfo property)
    {
        if (!property.PropertyType.IsPrimitive)
        {
            // Check if there is a Constructor which does not need any Attributes
            ConstructorInfo[] constructors = property.PropertyType.GetConstructors();
            ConstructorInfo? constructorWithNoParams = null;
            if ((constructorWithNoParams = constructors.Where(c => c.GetParameters().Length == 0).FirstOrDefault()) != null)
            {
                // found a Constructor with no Params
                // Now Check if any of the Properties of the Class, can be set and read.
                foreach (PropertyInfo propertyOfClass in property.PropertyType.GetProperties())
                {
                    if (propertyOfClass.CanRead && propertyOfClass.CanWrite)
                    {
                        return false; // Property does not need an XmlAttribute but a new Class definition
                    }
                }
            }
        }
        return true;
    }
    /// <summary>
    /// Will be executed when the PreviewContent was changed by the PreviewViewModel.
    /// <b>Important this Method must be called by each class which change the Preview Content.</b>
    /// This Event is needed, because other Views like the ViewHierarchy View has to reload the Tree View.
    /// </summary>
    public void OnPreviewContentChanged()
    {
        if (ViewHierarchyViewModel != null) ViewHierarchyViewModel.Reload();
    }
    /// <summary>
    /// Event which will be called by the ViewHierarchyViewModel 
    /// </summary>
    public void OnSelectedControlChanged()
    {
        if (PropertiesViewModel != null && SelectedControl != null)
        {
            // it is necessary to find the correct XmlControl for the given Control => see V1.0.16 (Issue #19)
            XmlControl? xmlControl = RootConnectedNode.Find(SelectedControl);
            if(xmlControl != null)
                PropertiesViewModel.RenderForControl(xmlControl);
        }
    }

    public void StartDrag(Control? control)
    {
        CurrentControl = control;
        Operation = EDragAndDropOperation.DropObjectToPreview;
    }

    public void DraggingPaused()
    {
        Operation = EDragAndDropOperation.Paused;
    }
    public void EndDrag()
    {
        CurrentControl = null;
        Operation = EDragAndDropOperation.None;
    }
}