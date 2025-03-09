using System.Reactive;
using BoTech.DesignerForAvalonia.Views;
using BoTech.DesignerForAvalonia.Views.Abstraction;
using ReactiveUI;

namespace BoTech.DesignerForAvalonia.ViewModels.Abstraction;

public class CloseablePageViewModel<T> : ViewModelBase  where T : CloseablePageCodeBehind
{
    public ReactiveCommand<Unit,Unit> CloseCommand { get; set;  }

    public CloseablePageViewModel(T codeBehind)
    {
        if(codeBehind is null) throw new System.ArgumentNullException(nameof(codeBehind));
        CloseCommand = ReactiveCommand.Create(() =>
        {
            codeBehind.Close();
        });
    }
}