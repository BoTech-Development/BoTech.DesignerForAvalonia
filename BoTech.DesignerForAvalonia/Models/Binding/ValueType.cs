namespace BoTech.DesignerForAvalonia.Models.Binding;

public enum ValueType
{
    /// <summary>
    /// When no FallbackValue is defined.
    /// </summary>
    None,
    /// <summary>
    /// When the fallbackvalue is defined through another Binding.
    /// </summary>
    Binding,
    /// <summary>
    /// An direct value assigment => {Binding Text, FallbackValue=Hello}
    /// </summary>
    Value
}