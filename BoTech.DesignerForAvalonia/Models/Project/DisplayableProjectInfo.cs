using System.Reactive;
using BoTech.DesignerForAvalonia.ViewModels;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.Models.Project;
/// <summary>
/// This class can be used to show the Project label on the first view.
/// </summary>
public class DisplayableProjectInfo 
{
   
    public bool Red { get;  set; }
    public bool Pink { get;  set; }
    public bool Purple { get;  set; }
    public bool Violet { get;  set; }
    public bool Indigo { get;  set; }
    public bool Blue { get;  set; }
    public bool LightBlue { get;  set; }
    public bool Cyan { get;  set; }
    public bool Teal { get;  set; }
    public bool Green { get;  set; }
    public bool LightGreen { get;  set; }
    public bool Lime { get;  set; }
    public bool Yellow { get;  set; }
    public bool Amber { get;  set; }
    public bool Orange { get;  set; }
    public bool Grey { get;  set; }
    public bool White { get;  set; }
    
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