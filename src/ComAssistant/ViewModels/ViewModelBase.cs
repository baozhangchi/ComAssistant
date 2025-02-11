using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace ComAssistant.ViewModels;

internal class ViewModelBase : ObservableObject, IViewModel
{
    public Control? View { get; private set; }

    void IViewModel.Attach(Control view)
    {
        View = view;
        if (view.IsLoaded)
            OnViewLoaded();
        else
            view.Loaded += View_Loaded;
        view.Unloaded += View_Unloaded;
    }

    private void View_Unloaded(object? sender, RoutedEventArgs e)
    {
        if (sender is Control view)
        {
            view.Unloaded -= View_Unloaded;
            WeakReferenceMessenger.Default.UnregisterAll(this);
            OnViewUnloaded();
        }
    }

    protected virtual void OnViewUnloaded()
    {
    }

    private void View_Loaded(object? sender, RoutedEventArgs e)
    {
        if (sender is Control view)
        {
            view.Loaded -= View_Loaded;
            OnViewLoaded();
        }
    }

    protected virtual void OnViewLoaded()
    {
    }
}

internal interface IViewModel
{
    Control? View { get; }
    void Attach(Control view);
}