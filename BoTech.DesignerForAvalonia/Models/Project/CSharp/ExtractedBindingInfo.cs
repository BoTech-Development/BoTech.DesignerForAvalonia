using Avalonia.Data;
using BoTech.DesignerForAvalonia.Models.Binding;

namespace BoTech.DesignerForAvalonia.Models.Project.CSharp;

public class ExtractedBindingInfo
{
    /// <summary>
    /// The name of the source property to be bound.
    /// </summary>
    public string Path { get; set; }
    /// <summary>
    /// Type of the Mode.
    /// </summary>
    public ValueType ModeValueType { get; private set; } = ValueType.None;

    private object? _mode;
    /// <summary>
    /// The synchronization direction of the binding.
    /// </summary>
    public object? Mode
    {
        get => _mode;
        set
        {
            _mode = value;
            if(value is BindingMode) ModeValueType = ValueType.Value;
            else ModeValueType = ValueType.Binding;
        }
    }

    /// <summary>
    /// Priority of the property setter.
    /// </summary>
    public BindingPriority Priority { get; set; }
    /// <summary>
    /// Type of the <see cref="RelativeSource"> RelativeSource.</see>
    /// </summary>
    public ValueType RelativeSourceValueType { get; private set; }
    /// <summary>
    /// Describes where the property to which the binding refers can be found. 
    /// </summary>
    public ExtractedRelativeSource RelativeSource { get; set; }  = new ExtractedRelativeSource(); 
    /// <summary>
    /// Type of the <see cref="ElementName"> ElementName.</see>
    /// </summary>
    public ValueType ElementNameValueType { get; private set; } = ValueType.None;

    private object? _elementName;
    /// <summary>
    /// Can be set to the name of another control when the binding refers to a property of this control.
    /// For instance: #MyControl.Text and {Binding Text, ElementName=MyControl} are the same.
    /// </summary>
    public object? ElementName 
    { 
        get => _elementName;
        set
        {
            _elementName = value;
            if(value is string) ElementNameValueType = ValueType.Value;
            else ElementNameValueType = ValueType.Binding;
        } 
    }
    /// <summary>
    /// Type of the <see cref="FallbackValue"> FallbackValue.</see>
    /// </summary>
    public ValueType FallBackValueType { get; private set; } = ValueType.None;

    private object? _fallbackValue;
    /// <summary>
    /// The Value which will be injected into the property when the referenced property is not reachable or null.
    /// </summary>
    public object? FallbackValue
    {
        get => _fallbackValue;
        set
        {
            _fallbackValue = value;
            if(value is string) FallBackValueType = ValueType.Value;
            else FallBackValueType = ValueType.Binding;
        }
    }
    /// <summary>
    /// Type of the <see cref="TargetNullValue"> TargetNullValue.</see>
    /// </summary>
    public ValueType TargetNullValueType { get; private set; } = ValueType.None;

    private object? _targetNullValue;
    /// <summary>
    /// Will be applied when the refenced property of this binding is null.
    /// </summary>
    public object? TargetNullValue
    {
        get => _targetNullValue;
        set
        {
            _targetNullValue = value;
            if(value is string) TargetNullValueType = ValueType.Value;
            else TargetNullValueType = ValueType.Binding;
        }
    }
    /// <summary>
    /// Type of the <see cref="UpdateSourceTrigger"> UpdateSourceTrigger. </see>
    /// </summary>
    public ValueType UpdateSourceTriggerValueType { get; private set; } = ValueType.None;

    private object? _updateSourceTrigger;
    /// <summary>
    /// Set when the binding should be updated.
    /// NOTE: Some parts may not have supported in this Version. For Instance Explicit.
    /// </summary>
    public object UpdateSourceTrigger
    {
        get => _updateSourceTrigger;
        set
        {
            _updateSourceTrigger = value;
            if(value is string) UpdateSourceTriggerValueType = ValueType.Value;
            else UpdateSourceTriggerValueType = ValueType.Binding;
        }
    }
    
}


