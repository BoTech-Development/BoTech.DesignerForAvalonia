using System.Reactive;
using Avalonia.Controls;
using Avalonia.Media;
using DialogHostAvalonia;
using Material.Icons;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.ViewModels.Editor.Dialogs;
/// <summary>
/// A Dialog View with basic functionality: Close and Save Button. Content can be set Dynamically.
/// </summary>
public class GenericDialogViewModel : ViewModelBase
{
    private Control _content;

    public Control Content
    {
        get => _content; 
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }
    public MaterialIconKind Icon { get; set; }
    public Brush IconColor { get; set; }
    public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; set; }
    public ReactiveCommand<Unit, Unit> HelpCommand { get; set; }

    public GenericDialogViewModel()
    {
        CloseCommand = ReactiveCommand.Create(() =>
        {
            DialogHost.Close("MainDialogHost");
        });
        
    }
}