using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Timers;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace ComAssistant.ViewModels;

internal partial class MainViewModel : ViewModelBase
{
    private readonly Timer _timer;
    [ObservableProperty] private RelayCommand _addComDebugViewCommand;
    [ObservableProperty] private ObservableCollection<ComDebugViewModel> _comDebugViewModels;

    [ObservableProperty] private string[] _ports = [];
    [ObservableProperty] private RelayCommand<ComDebugViewModel> _removeComDebugViewCommand;
    [ObservableProperty] private List<string> _usedPorts = new();

    public MainViewModel()
    {
        WeakReferenceMessenger.Default.Register<MainViewModel, string, string>(this, nameof(PortUsed), PortUsed);
        WeakReferenceMessenger.Default.Register<MainViewModel, string, string>(this, nameof(PortFree), PortFree);
        ComDebugViewModels = new ObservableCollection<ComDebugViewModel>();
        _timer = new Timer(100);
        _timer.Elapsed += Timer_Elapsed;
        AddComDebugViewCommand = new RelayCommand(AddComDebugView, CanAddComDebugView);
        RemoveComDebugViewCommand = new RelayCommand<ComDebugViewModel>(RemoveComDebugView);
        _timer.Start();
    }

    private void RemoveComDebugView(ComDebugViewModel? viewModel)
    {
        if (viewModel != null)
        {
            if (viewModel.DisconnectCommand.CanExecute(null)) viewModel.DisconnectCommand?.Execute(null);
            ComDebugViewModels.Remove(viewModel);
        }
    }

    private void PortFree(MainViewModel recipient, string message)
    {
        UsedPorts.Remove(message);
        foreach (var viewModel in ComDebugViewModels) viewModel.SetFreePorts(Ports.Except(UsedPorts).ToArray());
    }

    private void PortUsed(MainViewModel recipient, string message)
    {
        UsedPorts.Add(message);
        foreach (var viewModel in ComDebugViewModels) viewModel.SetFreePorts(Ports.Except(UsedPorts).ToArray());
    }

    private bool CanAddComDebugView()
    {
        return Ports.Any() && Ports.Length != ComDebugViewModels.Count;
    }

    private void AddComDebugView()
    {
        var viewModel = new ComDebugViewModel();
        viewModel.SetFreePorts(Ports.Except(UsedPorts).ToArray());
        ComDebugViewModels.Add(viewModel);
    }

    partial void OnPortsChanged(string[] value)
    {
        WeakReferenceMessenger.Default.Send(value, "PortsRefresh");
    }

    private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        _timer.Stop();
        Dispatcher.UIThread.Invoke(() =>
        {
            Ports = SerialPort.GetPortNames();
            AddComDebugViewCommand.NotifyCanExecuteChanged();
        });
        _timer.Start();
    }
}