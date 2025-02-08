using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;
using CommunityToolkit.Mvvm.ComponentModel;
using System.IO.Ports;
using System.Linq;
using CommunityToolkit.Mvvm.Input;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Avalonia.Threading;

namespace ComAssistant.ViewModels;

internal partial class MainViewModel : ViewModelBase
{
    private readonly Timer _timer;

    [ObservableProperty] private string[] _ports = [];
    [ObservableProperty] private List<string> _usedPorts = new();
    [ObservableProperty] private RelayCommand _addComDebugViewCommand;
    [ObservableProperty] private ObservableCollection<ComDebugViewModel> _comDebugViewModels;

    public MainViewModel()
    {
        WeakReferenceMessenger.Default.Register<MainViewModel, string, string>(this, nameof(PortUsed), PortUsed);
        WeakReferenceMessenger.Default.Register<MainViewModel, string, string>(this, nameof(PortFree), PortFree);
        ComDebugViewModels = new ObservableCollection<ComDebugViewModel>();
        _timer = new Timer(100);
        _timer.Elapsed += Timer_Elapsed;
        AddComDebugViewCommand = new RelayCommand(AddComDebugView, CanAddComDebugView);
        _timer.Start();
    }

    private void PortFree(MainViewModel recipient, string message)
    {
        UsedPorts.Remove(message);
    }

    private void PortUsed(MainViewModel recipient, string message)
    {
        UsedPorts.Add(message);
    }

    private bool CanAddComDebugView()
    {
        return Ports.Any() && Ports.Length != ComDebugViewModels.Count;
    }

    private void AddComDebugView()
    {
        var viewModel = new ComDebugViewModel();
        viewModel.Ports = Ports;
        viewModel.UsedPorts = UsedPorts;
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