using System;
using Avalonia.Data;

namespace BoTech.DesignerForAvalonia.Models.Binding;

public class ExtractedRelativeSource
{

    private int _ancestorLevel = 1;
    /// <summary>
    /// Type of the <see cref="RelativeSource"> RelativeSource.</see>
    /// </summary>
    public ValueType AncestorLevelValueType { get; private set; }
    /// <summary>
    /// Gets the level of ancestor to look for when in <see cref="RelativeSourceMode.FindAncestor"/>  mode.
    /// </summary>
    /// <remarks>
    /// Use the default value of 1 to look for the first ancestor of the specified type.
    /// </remarks>
    public object? AncestorLevel 
    { 
        get { return _ancestorLevel; }
        set
        {
            if (value is int ancestorLevel)
            {
                if (ancestorLevel <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "AncestorLevel may not be set to less than 1.");
                _ancestorLevel = ancestorLevel;
                AncestorLevelValueType = ValueType.Value;
            }
            else
            {
                AncestorLevel = value;
                AncestorLevelValueType = ValueType.Binding;
            }
        }
    }
    /// <summary>
    /// Type of the <see cref="RelativeSource"> RelativeSource.</see>
    /// </summary>
    public ValueType AncestorTypeValueType { get; private set; }

    private object? _ancestorType;

    /// <summary>
    /// Gets the type of ancestor to look for when in <see cref="RelativeSourceMode.FindAncestor"/>  mode.
    /// </summary>
    public object? AncestorType
    {
        get => _ancestorType;
        set
        {
            if (value is string)
                AncestorTypeValueType = ValueType.Value;
            else
                AncestorTypeValueType = ValueType.Binding;
            _ancestorType = value;
        }
    }
    /// <summary>
    /// Type of the <see cref="RelativeSource"> RelativeSource.</see>
    /// </summary>
    public ValueType ModeValueType { get; private set; }

    private object? _mode;
    /// <summary>
    /// Gets or sets a value that describes the type of relative source lookup.
    /// </summary>
    public object? Mode
    {
        get => _mode;
        set
        {
            if (value is RelativeSourceMode)
                ModeValueType = ValueType.Value;
            else
                ModeValueType = ValueType.Binding;
            _mode = value;
        }
    } //RelativeSourceMode
    /// <summary>
    /// Type of the <see cref="RelativeSource"> RelativeSource.</see>
    /// </summary>
    public ValueType TreeValueType { get; private set; }

    private object? _tree;
    /// <summary>
    /// On which tree type the binding should be applied.
    /// </summary>
    public object? Tree
    {
        get => _tree;
        set
        {
            if (value is TreeType tree)
                TreeValueType = ValueType.Value;
            else
                TreeValueType = ValueType.Binding;
            _tree = value;
        }
    } // TreeType
    
}