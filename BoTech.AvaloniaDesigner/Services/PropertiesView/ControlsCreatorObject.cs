using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using BoTech.AvaloniaDesigner.Models.XML;
using BoTech.AvaloniaDesigner.ViewModels.Editor;
using DynamicData;

namespace BoTech.AvaloniaDesigner.Services.PropertiesView;
/// <summary>
/// This class can be used to create editable Controls for an unknown Type.
/// The User can use any constructor of this class to create a new Object.
/// This Object which was visually created can be Injected in a Property
/// </summary>
public class ControlsCreatorObject
{
    private ConstructorModel _constructorModel;
    private XmlControl _referencedXmlControl; 
    public Expander EditableControls { get; set; }



    public ControlsCreatorObject(PropertyInfo propertyInfo, XmlControl referencedXmlControl, PropertiesViewModel.TabContent tabContent, string viewTemplateName)
    {
        _referencedXmlControl = referencedXmlControl;
        _constructorModel = new ConstructorModel(propertyInfo.PropertyType, propertyInfo.Name, _referencedXmlControl, tabContent, viewTemplateName);
        EditableControls = _constructorModel.Render();
    }

    public void Rerender()
    {
        EditableControls = _constructorModel.Rerender();;
    }
    /// <summary>
    /// Creates an editable control for a specific parameter and its current value, adding it to the provided expander content.
    /// It determines the type of the parameter and generates an appropriate control accordingly.
    /// </summary>
    /// <param name="parameter">The parameter information containing details such as name and type of the parameter.</param>
    /// <param name="currentValue">The current value of the parameter.</param>
    /// <param name="expanderContent">The StackPanel to which the created control will be added as a child element.</param>
    /// <param name="model">The Model which is connected to the Control.</param>
    private static void CreateEditableControl(ParameterInfo parameter, object currentValue, StackPanel expanderContent, ConstructorModel model)
    {
        switch (parameter.ParameterType.Name)
         {
                    
                     // All possible bool based Types:
             case "Boolean": 
                 expanderContent.Children.Add(
                     ControlsCreatorObject.CreateEditableControlForBoolean(parameter, currentValue, model));
                 break;
             case "bool": 
                 expanderContent.Children.Add(
                     ControlsCreatorObject.CreateEditableControlForBoolean(parameter, currentValue, model));
                 break;
                    // All possible Integer based Types. 
                        
             case "Int16":
                 expanderContent.Children.Add(
                     ControlsCreatorObject.CreateControlFloatingForInteger(parameter, currentValue, model));
                 break;
             case "UInt16":  
                 expanderContent.Children.Add(
                     ControlsCreatorObject.CreateControlFloatingForInteger(parameter, currentValue, model));
                 break;
             case "Int32": 
                 expanderContent.Children.Add(
                     ControlsCreatorObject.CreateControlFloatingForInteger(parameter, currentValue, model));
                 break;
             case "UInt32": 
                 expanderContent.Children.Add(
                     ControlsCreatorObject.CreateControlFloatingForInteger(parameter, currentValue, model));
                 break;
             case "Int64":
                 expanderContent.Children.Add(
                     ControlsCreatorObject.CreateControlFloatingForInteger(parameter, currentValue, model));
                 break;
             case "UInt64": 
                 expanderContent.Children.Add(
                     ControlsCreatorObject.CreateControlFloatingForInteger(parameter, currentValue, model));
                 break;
                        
                    // All Floating-Point primitive Types:
                        
             case "Single":
                 if (!float.IsInfinity((float)currentValue))
                 {
                     expanderContent.Children.Add(
                         ControlsCreatorObject.CreateEditableControlFloatingPoint(parameter, currentValue, model));
                     break;
                 }
                 expanderContent.Children.Add(new TextBlock()
                 {
                     Text = "Can not display: " + parameter.Name + "because its +|- infinite.",
                 });
                 break;
             case "Double":
                 if (!double.IsInfinity((double)currentValue))
                 {
                     expanderContent.Children.Add(
                         ControlsCreatorObject.CreateEditableControlFloatingPoint(parameter, currentValue, model));
                     break;
                 }
                 expanderContent.Children.Add(new TextBlock()
                 {
                     Text = "Can not display: " + parameter.Name + "because its +|- infinite.",
                 });
                 break;
             case "Decimal": 
                 expanderContent.Children.Add(
                     ControlsCreatorObject.CreateEditableControlFloatingPoint(parameter, currentValue, model));
                 break;
                            
                            
                    // Primitive Type string and Class String are handled the same way:
             case "string": 
                 expanderContent.Children.Add(
                     ControlsCreatorObject.CreateEditableControlForString(parameter, "", model));
                 break;
             case "String": 
                 expanderContent.Children.Add(
                     ControlsCreatorObject.CreateEditableControlForString(parameter, "", model));
                 break;
                    
                        
         }
    }
    private static Control CreateEditableControlForBoolean(ParameterInfo parameterInfo, object currentValue, ConstructorModel model)
    {
        CheckBox cb = new CheckBox()
        {
            Content = parameterInfo.Name,
            IsChecked = (bool)currentValue!,
        };
       
        cb.IsCheckedChanged += (s, e) =>
        {
            if (parameterInfo.Name != null)
                model.OnParameterForConstructorChanged(parameterInfo.Name, cb.IsChecked);
        };
        return cb;
    }

    /// <summary>
    /// External Method which is an extraction for all Integer Based Types
    /// </summary>
    /// <param name="parameterInfo"></param>
    /// <param name="currentValue"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    private static Control CreateControlFloatingForInteger(ParameterInfo parameterInfo, object currentValue, ConstructorModel model)
    {
        NumericUpDown numericUpDown = new NumericUpDown()
        {
            Text = parameterInfo.Name,
            FormatString = "0",
            Minimum = 0,
            Value = Convert.ToDecimal(currentValue),
            Increment = 1,
        };
        numericUpDown.ValueChanged += (sender, args) =>
        {
            if (parameterInfo.Name != null)
                model.OnParameterForConstructorChanged(parameterInfo.Name, numericUpDown.Value);
        };
        
        return ControlsCreatorObject.AddEditableControlToStackPanel(numericUpDown, parameterInfo);
    }

    /// <summary>
    /// External Method which is an extraction for all Floating Point Based Types
    /// </summary>
    /// <param name="parameterInfo"></param>
    /// <param name="currentValue"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    private static Control CreateEditableControlFloatingPoint(ParameterInfo parameterInfo, object currentValue, ConstructorModel model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        NumericUpDown numericUpDown = new NumericUpDown()
        {
            Text = parameterInfo.Name,
            FormatString = "0.00",
            Minimum = 0,
            Value = Convert.ToDecimal(currentValue)!,
            Increment = 1,
        };
        numericUpDown.ValueChanged += (s, e) =>
        {
            if (parameterInfo.Name != null)
                model.OnParameterForConstructorChanged(parameterInfo.Name, 
                    Convert.ChangeType(numericUpDown.Value, parameterInfo.ParameterType));
            // Type Conversion is needed because this Method is used by multiple Types like single, float, double and decimal
        };
        return ControlsCreatorObject.AddEditableControlToStackPanel(numericUpDown, parameterInfo);
    }

    /// <summary>
    /// External Method which is an extraction for all String Based Types
    /// </summary>
    /// <param name="parameterInfo"></param>
    /// <param name="currentValue"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    private static Control CreateEditableControlForString(ParameterInfo parameterInfo, object currentValue, ConstructorModel model)
    {
        TextBox tb = new TextBox()
        {
            Text = (string)currentValue!,
        };
        tb.TextChanged += (s, e) =>
        {
            if (parameterInfo.Name != null)
                model.OnParameterForConstructorChanged(parameterInfo.Name, tb.Text);
        };
        // EditorController.OnPropertyInPropertiesViewChanged(control, propertyInfo.Name, tb.Text);
        return ControlsCreatorObject.AddEditableControlToStackPanel(tb, parameterInfo);
    }

    /// <summary>
    /// This Method is a Helper Method to add the Property name to the Control. 
    /// </summary>
    /// <param name="control"></param>
    /// <param name="propertyInfo"></param>
    /// <returns></returns>
    private static StackPanel AddEditableControlToStackPanel(Control control, ParameterInfo propertyInfo)
    {
        StackPanel stackPanel = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 3, 0, 0)
        };
        stackPanel.Children.Add(new TextBlock()
        {
            Text = propertyInfo.Name + ":",
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0,0,5, 0),
            FontWeight = FontWeight.Bold
        });
        stackPanel.Children.Add(control);
        return stackPanel;
    }

    public class ConstructorModel
    {
        /// <summary>
        /// The Control and XmlNode which the User has Selected.
        /// </summary>
        private XmlControl ReferencedXmlControl { get; set; } 
        private PropertiesViewModel.TabContent TabContent { get; set; }
        private string ViewTemplateName { get; set; }
        /// <summary>
        /// The Selected Constructor definition
        /// </summary>
        private ConstructorInfo SelectedConstructor { get; set; }
        /// <summary>
        /// All available Constructors for this Type
        /// </summary>
        private ConstructorInfo[] AvailableConstructors { get; set; }
        /// <summary>
        /// This list store all the primitive or non-primitive objects, which are needed for the Selected Constructor.
        /// For example if the Constructor has an int in its definition the Integer object will store in this List.
        /// </summary>
        private List<object> ParameterValues { get; set; } = new List<object>();
        /// <summary>
        /// A Constructor which does has one or more non-primitive Types, has one or a bunch of nested Controls to represent their visibility.
        /// </summary>
        private List<ConstructorModel> Children { get; set; } = new List<ConstructorModel>();
        /// <summary>
        /// The Parent Model, which is set when this instance is a Part of a Children Collection in another object.
        /// It is used to Render all changes from the bottom of the tree up to the master Object.
        /// </summary>
        private ConstructorModel? Parent { get; set; } 
        private ComboBox _constructorSelectionBox = new ComboBox();
        private Type _type;
        /// <summary>
        /// This Name is the Name of the Parameter in the selected Constructor of the Parent Model.
        /// It is stored to update the correct value when this Value has changed.
        /// </summary>
        private string? _nameOfTypeInConstructor = null;

        /// <summary>
        /// Represents the name of a property within a control that may be used for identification of the Property in the Control.
        /// For Instance, it is used to update the Property in the Control.
        /// It is only stored in the most parent model, because the master model can only set the value because it has all the necessary Properties.
        /// </summary>
        private string? _nameOfPropertyInControl = null;
     
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="referencedControl"></param>
        /// <param name="tabContent">The TabContent which will handle the Rendering event.</param>
        /// <param name="viewTemplateName">The Name of the Parent ViewTemplate.</param>
        public ConstructorModel(Type type, string nameOfPropertyInControl, XmlControl referencedXmlControl, PropertiesViewModel.TabContent tabContent, string viewTemplateName)
        {   
            _type = type;
            _nameOfPropertyInControl = nameOfPropertyInControl;
            ReferencedXmlControl = referencedXmlControl;
            AvailableConstructors = type.GetConstructors();
            SelectedConstructor = AvailableConstructors[0];
            ParameterValues = new List<object>();
            TabContent = tabContent;
            ViewTemplateName = viewTemplateName;
            Create();
        }
        public ConstructorModel(Type type, string nameOfTypeInConstructor, ConstructorModel parent, XmlControl referencedXmlControl, PropertiesViewModel.TabContent tabContent, string viewTemplateName)
        {   
            _nameOfTypeInConstructor = nameOfTypeInConstructor;
            _type = type;
            ReferencedXmlControl = referencedXmlControl;
            AvailableConstructors = type.GetConstructors();
            SelectedConstructor = AvailableConstructors[0];
            ParameterValues = new List<object>();
            Parent = parent;
            TabContent = tabContent;
            ViewTemplateName = viewTemplateName;
            Create();
        }
        /// <summary>
        /// Creates all Children for the Selected Constructor if it is necessary and store all default values for in the ParameterValues.
        /// </summary>
        private void Create()
        {
            foreach (ParameterInfo parameter in SelectedConstructor.GetParameters())
            {
                if (ControlsCreator.SupportedPrimitiveTypes.Contains(parameter.ParameterType.Name))
                {
                    // Create a default value for the Parameter of the Constructor
                    ParameterValues.Add(Activator.CreateInstance(parameter.ParameterType)!);
                }
                else
                {
                    // The Type is not Supported and need an editable Control for its Constructor
                    Children.Add(new ConstructorModel(parameter.ParameterType, parameter.Name, this, ReferencedXmlControl, TabContent, ViewTemplateName));
                    // Create a default Value for this non-Primitive Parameter.
                    // It is neccessary to create a new Item in the List, because the Child object will set it.
                    ParameterValues.Add(null);
                }
            }
        }
        /// <summary>
        /// Renders all Children and Itself recursively.
        /// This Method creates the Expander with a Combobox to Select between the different Constructors and all Editable Controls for each Parameter in the Constructor
        /// </summary>
        /// <returns>The Rendered Output.</returns>
        public Expander Render()
        {
            List<ComboBoxItem> comboBoxItems = new List<ComboBoxItem>();
            // Create ComboBox with all Constructors of the given Type.
            // The ComboBox will be used to select the Constructor which should be used to create a new Object.
            foreach (ConstructorInfo constructor in AvailableConstructors)
            {
                string constructorDefinition = ConvertConstructorInfoToString(constructor);
                comboBoxItems.Add(new ComboBoxItem()
                {
                    Content = constructorDefinition,
                });
            }

            ComboBox cb = new ComboBox()
            {
                ItemsSource = comboBoxItems
            };
            
            // Calculate Selected Index
            int selectedIndex = 0;
            foreach (ComboBoxItem comboBoxItem in comboBoxItems)
            {
                if(comboBoxItem.Content != null)
                {
                    string? selectedConstructorDefinition = comboBoxItem.Content.ToString();
                    if (selectedConstructorDefinition == ConvertConstructorInfoToString(SelectedConstructor))
                    {
                        cb.SelectedIndex = selectedIndex;
                        break;
                    }
                    selectedIndex++;
                }
            }
            
         
            cb.SelectionChanged += OnSelectedConstructorChanged;
            // Store the Combobox in a local var evaluation purposes in the event. 
            _constructorSelectionBox = cb;
            
            StackPanel expanderContent = new StackPanel()
            {
                Orientation = Orientation.Vertical
            };
            Expander expander = new Expander()
            {
                Header = new StackPanel()
                {
                    Children =
                    {
                        new TextBlock()
                        {
                            // The ?? Checks which of the two values is null and returns the value which isn't null
                            Text = _nameOfPropertyInControl ?? _nameOfTypeInConstructor,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(0,0,5, 0),
                            FontWeight = FontWeight.Bold
                        },
                        cb,
                    },
                    Orientation = Orientation.Horizontal
                },
                Content = expanderContent,
                Margin = new Thickness(0,5,0,0)
            };
            
            int currentChildIndex = 0;
            int currentParameterIndex = 0;
            foreach (ParameterInfo parameter in SelectedConstructor.GetParameters())
            {
                if (ControlsCreator.SupportedPrimitiveTypes.Contains(parameter.ParameterType.Name) ||
                    ControlsCreator.SupportedAvaloniaTypes.Contains(parameter.ParameterType.Name))
                {
                    // Editable Control for this Parameter can be created with the CreateEditableControl Method
                    ControlsCreatorObject.CreateEditableControl(parameter, ParameterValues[currentParameterIndex],
                        expanderContent, this);
                }
                else
                {
                    // The Parameter Type is not Supported. 
                    // Now the Editable Control can be created with one of the Child Models.
                    expanderContent.Children.Add(Children[currentChildIndex].Render());
                    currentChildIndex++;
                }

                currentParameterIndex++;
            }
            return expander;
        }

        public Expander Rerender()
        {
            // It is necessary to check if this Model needs to rerender because when there a multiple StandardViewTemplates assigned to the ViewTemplate all Standard ViewTemplates will be rendered.
            return Render();
        }

        
        private void OnSelectedConstructorChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_constructorSelectionBox.SelectedItem != null)
            {
                ComboBoxItem selectedItem = ((ComboBoxItem)_constructorSelectionBox.SelectedItem);
                if (selectedItem.Content != null)
                {
                    string? selectedConstructorDefinition = selectedItem.Content.ToString();

                    // Create a List of all definitions
                    List<string> constructorDefinitions = new List<string>();
                    foreach (ConstructorInfo constructor in AvailableConstructors)
                    {
                        constructorDefinitions.Add(ConvertConstructorInfoToString(constructor));
                    }

                    int index = constructorDefinitions.FindIndex(c => c == selectedConstructorDefinition);
                    if (index != -1)
                    {
                        SelectedConstructor = AvailableConstructors[index];
                        ParameterValues.Clear(); // Reset all Selected Values
                        Create(); // Update all Children and all editable Controls for this Constructor
                        // Call the event Handling Method in the TabContent class to Rerender everything
                        TabContent.RenderOnlySelectedTemplate(ViewTemplateName, ReferencedXmlControl);
                    }
                }
            }
        }
        /// <summary>
        /// This Method Converts a Constructor Info in a readable String of the following schema: new {ClassNameAndNamespace} ( {ParameterTypeName} {ParameterName}, ...)
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        private string ConvertConstructorInfoToString(ConstructorInfo constructor)
        {
            string constructorDefinition = "new " + constructor.DeclaringType?.FullName + " (";
            ParameterInfo[] parameters = constructor.GetParameters();
            foreach (ParameterInfo parameter in parameters)
            {
                constructorDefinition += parameter.ParameterType.Name + " " + parameter.Name + ", ";
            }
            constructorDefinition += ")";
            return constructorDefinition;
        }
        /// <summary>
        /// This event will call when the user changes one of the Editable Controls which are assigned to this Instance.
        /// If this Instance has a Parent it will update the new Value in the Parent instance.
        /// As a result the "complete" tree of ConstructorModels will be updated from the current node.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="newValue"></param>
        public void OnParameterForConstructorChanged(string parameterName, object? newValue)
        {
            // Get the Index for the ParameterValues List
            int index = SelectedConstructor.GetParameters().ToList().FindIndex(p => p.Name == parameterName);
            if (index != -1)
            {
                ParameterValues[index] = newValue;
            }
            /*
             * + Thickness
             *  + int
             *  + Rect
             *      + int 1 => 3
             *      + int 2
             *      + int 5
             *      + int 10
             */
            /*
             * How the sequence works:
             * 1. Control call the OnParameterForConstructorChanged Method
             *      + This Method only stores the new Value and creates a new Instance of its Type (_type)
             * 
             * 2. The Child Control call the OnChildModelChanged Method in the Parent Model.
             * 
             */
            
            if (Parent != null)
            {
                Parent.OnChildModelChanged(this, _nameOfTypeInConstructor, Activator.CreateInstance(_type, ParameterValues.ToArray()));
            }
            else
            {
                // This Model is the Master Model so we can create a new instance for the Property in the selected Control
                if (_nameOfPropertyInControl != null)
                {
                    object? nextInstance = SelectedConstructor.Invoke(ParameterValues.ToArray());
                    //object? nextInstance = Activator.CreateInstance(_type, ParameterValues.ToArray());
                    ReferencedXmlControl.Control.GetType().GetProperty(_nameOfPropertyInControl)?.SetValue(ReferencedXmlControl.Control, nextInstance);
                }
            }
        }
        /// <summary>
        /// Updates the Parameter (parameterName) for the selected Constructor.
        /// If its necessary it will notify all Parent Models by calling the OnChildModelChanged Method on the Parent Model with a new Instance of the given Type which was created by the Selected Constructor.
        /// </summary>
        /// <param name="childModel">The child model that triggered the update.</param>
        /// <param name="parameterName">The name of the parameter in the current constructor model that needs to be updated.</param>
        /// <param name="newValue">The new value for the specified parameter.</param>
        public void OnChildModelChanged(ConstructorModel childModel, string? parameterName, object? newValue)
        {
            if (parameterName != null)
            {
                // Store the new Value...
                int index = SelectedConstructor.GetParameters().ToList().FindIndex(p => p.Name == parameterName);
                if (index != -1)
                {
                    ParameterValues[index] = newValue;
                }
                // Update all Parent Controls if it is necessary
                if (Parent != null)
                {
                    // Go one Step Higher in the Tree
                    object? nextInstance = SelectedConstructor.Invoke(ParameterValues.ToArray());
                    Parent.OnChildModelChanged(this, _nameOfTypeInConstructor, nextInstance);
                }
                else
                {
                    // We are at the Master Parent and can create a new Instance of the Selected Property:
                    // All the values which are stored in the ParameterValues List are used to Create a new Instance for the Property in the Selected Control
                    if (_nameOfPropertyInControl != null)
                    {
                        object? nextInstance = SelectedConstructor.Invoke(ParameterValues.ToArray());
                        ReferencedXmlControl.Control.GetType().GetProperty(_nameOfPropertyInControl)?.SetValue(ReferencedXmlControl.Control, nextInstance);
                    }
                }
            }
        }
    }
}