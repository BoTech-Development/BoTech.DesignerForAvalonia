using Avalonia.Controls;
using BoTech.AvaloniaDesigner.Views;
using ReactiveUI;

namespace BoTech.AvaloniaDesigner.ViewModels;

public class DialogWindowViewModel : ViewModelBase
{
    public static DialogWindowViewModel Instance { get; set; } = new DialogWindowViewModel();
    private UserControl _content;

    public UserControl Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }
    private DialogWindow _dialogWindow;
    private MainWindow _owner;
    /// <summary>
    /// Open the Dialog with the given Content
    /// </summary>
    /// <param name="content"></param>
    /// <param name="dialogWindow"></param>
    public async void ShowDialog(UserControl content)
    {
        Content = content; 
        _dialogWindow = new DialogWindow();
        _dialogWindow.DataContext = this;
        await _dialogWindow.ShowDialog(_owner);
    }
    /// <summary>
    /// Closes the Dialog
    /// </summary>
    public void CloseDialog()
    {
        _dialogWindow.Close();
    }

    public void SetOwner(MainWindow owner)
    {
        _owner = owner;
    }
}