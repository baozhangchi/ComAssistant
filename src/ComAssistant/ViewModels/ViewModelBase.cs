using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace ComAssistant.ViewModels;

internal class ViewModelBase : ObservableObject
{
    ~ViewModelBase()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
        OnClosing();
    }

    protected virtual void OnClosing()
    {
        
    }
}
