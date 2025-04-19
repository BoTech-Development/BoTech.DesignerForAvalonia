using System.Reactive;
using BoTech.DesignerForAvalonia.ViewModels;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.Models.Project;
/// <summary>
/// This class can be used to show the Project label on the first view.
/// </summary>
public class DisplayableProjectInfo
{

    public bool Red { get; set; } = true;
    public bool Pink { get;  set; } = false;
    public bool Purple { get;  set; } = false;
    public bool Violet { get;  set; } = false;
    public bool Indigo { get;  set; } = false;
    public bool Blue { get;  set; } = false;
    public bool LightBlue { get;  set; } = false;
    public bool Cyan { get;  set; } = false;
    public bool Teal { get;  set; } = false;
    public bool Green { get;  set; } = false;
    public bool LightGreen { get;  set; } = false;
    public bool Lime { get;  set; } = false;
    public bool Yellow { get;  set; } = false;
    public bool Amber { get;  set; } = false;
    public bool Orange { get;  set; } = false;
    public bool Grey { get;  set; } = false;
    public bool White { get;  set; } = false;
    
    public void SetColorByName(string colorName)
    {
        SetAllToLow();
        switch (colorName)
        {
            case "Red":
                Red = true;
                break;
            case "Pink":
                Pink = true;
                break;
            case "Purple":
                Purple = true;
                break;
            case "Violet":
                Violet = true;
                break;
            case "Indigo":
                Indigo = true;
                break;
            case "Blue":
                Blue = true;
                break;
            case "Cyan":
                Cyan = true;
                break;
            case "Teal":
                Teal = true;
                break;
            case "Green":
                Green = true;
                break;
            case "Lime":
                Lime = true;
                break;
            case "Yellow":
                Yellow = true;
                break;
            case "Amber":
                Amber = true;
                break;
            case "Orange":
                Orange = true;
                break;
            case "Grey":
                Grey = true;
                break;
            case "White":
                White = true;
                break;
        }
    }

    private void SetAllToLow()
    {
        Red = false;
        Pink = false;
        Purple = false;
        Violet = false;
        Indigo = false;
        Blue = false;
        LightBlue = false;
        Cyan = false;
        Teal = false;
        Green = false;
        LightGreen = false;
        Lime = false;
        Yellow = false;
        Grey = false;
        White = false;
    }
    
}