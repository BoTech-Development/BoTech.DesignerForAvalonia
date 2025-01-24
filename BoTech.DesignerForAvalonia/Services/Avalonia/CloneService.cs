using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BoTech.DesignerForAvalonia.Services.Avalonia;

public class CloneService
{
    public static object CloneProperty(PropertyInfo propertyInfo, object copy, object original)
    {
        // Cloning the Property
        if (HasTypeConstructorWithNoParams(propertyInfo.PropertyType))
        { 
            // Cloning all Properties:
            PropertyInfo[] properties = copy.GetType().GetProperties();
            List<PropertyInfo> originalProperties = original.GetType().GetProperties().ToList();
            foreach (PropertyInfo property in properties)
            {
                // When it is a primitive Type we can use the Parse Method of the primitive type to copy it
                if (property.PropertyType.IsPrimitive)
                {
                    PropertyInfo? originalProperty = originalProperties.Find(p => p.Name == property.Name);
                    if (originalProperty != null && originalProperty.GetValue(original) != null)
                    {
                        string originalString = originalProperty.GetValue(original).ToString();
                        switch (propertyInfo.PropertyType.Name)
                        {
                            case "Boolean":
                                property.SetValue(copy, Boolean.Parse(originalString));
                                break;
                            case "Byte":
                                property.SetValue(copy, Byte.Parse(originalString));
                                break;
                            case "SByte":
                                property.SetValue(copy, SByte.Parse(originalString));
                                break;
                            case "Char":
                                property.SetValue(copy, Char.Parse(originalString));
                                break;
                            case "Decimal": 
                                property.SetValue(copy, Decimal.Parse(originalString));
                                break;
                            case "Double":
                                property.SetValue(copy, Double.Parse(originalString));
                                break;
                            case "Single":
                                property.SetValue(copy, Char.Parse(originalString));
                                break;
                            case "Int32":
                                property.SetValue(copy, Int32.Parse(originalString));
                                break;
                            case "UInt32":
                                property.SetValue(copy, UInt32.Parse(originalString));
                                break;
                            case "IntPtr":
                                property.SetValue(copy, IntPtr.Parse(originalString));
                                break;
                            case "UIntPtr":
                                property.SetValue(copy, UIntPtr.Parse(originalString));
                                break;
                            case "Int64":
                                property.SetValue(copy, Int64.Parse(originalString));
                                break;
                            case "UInt64":
                                property.SetValue(copy, UInt64.Parse(originalString));
                                break;
                            case "Int16":
                                property.SetValue(copy, Int16.Parse(originalString));
                                break;
                            case "UInt16":
                                property.SetValue(copy, UInt16.Parse(originalString));
                                break;
                        }
                    }
                }
                else
                {
                    // Go step deeper until the Property is a primitive.
                    property.SetValue(copy, CloneProperty(property, copy, original));
                }
            }
            return copy;
        }
        
        // TODO: Copy all Types which have params in the constructors
        //ExecuteCloneThroughTheCtor(propertyInfo, copy, original);
        // For now just return null to short up the development cycle
        return null;
    }
    /// <summary>
    /// Checks if the given Type has a constructor which does not contain any params in it definition.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool HasTypeConstructorWithNoParams(Type type)
    {
        foreach (ConstructorInfo ctor in type.GetConstructors())
        {
            if(ctor.GetParameters().Length == 0) return true;
        }
        return false;
    }
    /// <summary>
    /// Make a deep copy by searching for the Properties which are needed to invoke the Constructor.
    /// For Instance when the given Type is Thickness. The Thickness class has the Properties Bottom, Left, Right and Top and a constructor (Thickness(double left, double top, double right, double bottom). 
    /// </summary>
    /// <param name="propertyInfo"></param>
    /// <param name="copy"></param>
    /// <param name="original"></param>
    private static void ExecuteCloneThroughTheCtor(PropertyInfo propertyInfo, object copy, object original)
    {
        // Search for the Constructor where all params exists as Properties in the Type 
        foreach (ConstructorInfo constructor in propertyInfo.PropertyType.GetConstructors())
        {
            List<PropertyInfo> propertyInfos = propertyInfo.PropertyType.GetProperties().ToList();
            List<object> paramValues = new List<object>();
            int parameterCount = 0;
            foreach (ParameterInfo parameter in constructor.GetParameters())
            {
                PropertyInfo property = propertyInfos.Find(p => p.Name == parameter.Name);
                if (property != null)
                {
                    parameterCount++;
                    paramValues.Add(property.GetValue(original, null));
                }
            }
            // All necessary params for the Constructor was found
            if (parameterCount == constructor.GetParameters().Length)
            {
                try
                {
                    object copyObj = constructor.Invoke(paramValues.ToArray());
                    // TODO: All Properties has to be updated
                    // Update the copy Value
                    propertyInfo.SetValue(copy, copyObj);
                }
                catch (Exception e)
                {
                    // Do nothing
                    continue;
                }
            }
        }   
    }
}