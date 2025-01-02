using BoTech.AvaloniaDesigner.Views;
using ReactiveUI;

namespace BoTech.AvaloniaDesigner.ViewModels;

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
    public int _currentPercentage = 0;
    public int CurrentPercentage
    {
        get => _currentPercentage;
        set => this.RaiseAndSetIfChanged(ref _currentPercentage, value);
    }
    
    public int _maximum = 100;
    public int Maximum
    {
        get => _maximum;
        set => this.RaiseAndSetIfChanged(ref _maximum, value);
    }
    private LoadingView _loadingView;
    private DialogWindowViewModel _dialogWindowViewModel;



    public void ShowLoadingDialog()
    {
        _loadingView = new LoadingView()
        {
            DataContext = this
        };
        DialogWindowViewModel.Instance.ShowDialog(_loadingView);

    }

    public void CloseLoadingDialog()
    {
        _dialogWindowViewModel.CloseDialog();
    }
}