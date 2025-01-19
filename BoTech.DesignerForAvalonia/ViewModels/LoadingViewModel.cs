using BoTech.DesignerForAvalonia.Views;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.ViewModels;

public class LoadingViewModel : ViewModelBase
{
    private string _statusText = "Init...";

    public string StatusText
    {
        get => _statusText;
        set => this.RaiseAndSetIfChanged(ref _statusText, value);
    }
    
    private string _subStatusText = "";

    public string SubStatusText
    {
        get => _subStatusText;
        set => this.RaiseAndSetIfChanged(ref _subStatusText, value);
    }
    
    private bool _isIndeterminate = true;

    public bool IsIndeterminate
    {
        get => _isIndeterminate;
        set => this.RaiseAndSetIfChanged(ref _isIndeterminate, value);
    }
    public int _current = 0;
    public int Current
    {
        get => _current;
        set => this.RaiseAndSetIfChanged(ref _current, value);
    }
    
    public int _maximum = 100;
    public int Maximum
    {
        get => _maximum;
        set => this.RaiseAndSetIfChanged(ref _maximum, value);
    }
}