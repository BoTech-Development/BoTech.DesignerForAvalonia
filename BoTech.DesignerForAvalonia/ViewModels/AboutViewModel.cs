using System.Diagnostics;
using System.Reactive;
using DialogHostAvalonia;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.ViewModels;

public class AboutViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> OpenWebsiteCommand { get; set; }
    public ReactiveCommand<Unit, Unit> OpenSupportWebsiteCommand { get; set; }
    public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }

    public AboutViewModel()
    {
        OpenWebsiteCommand = ReactiveCommand.Create(OpenWebsite);
        OpenSupportWebsiteCommand = ReactiveCommand.Create(OpenSupportWebsite);
        CloseCommand = ReactiveCommand.Create(Close);
    }

    private void OpenSupportWebsite()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://aka.botech.dev/go/Support/DesignerForAvalonia/",
            UseShellExecute = true
        });
    }

    private void OpenWebsite()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://www.botech.dev",
            UseShellExecute = true
        });
    }

    private void Close()
    {
        DialogHost.Close("MainDialogHost");
    }
}