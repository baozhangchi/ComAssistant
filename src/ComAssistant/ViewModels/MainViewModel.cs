using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Timers;
using Avalonia.Threading;
using ComAssistant.Lang;
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
        ComDebugViewModels = new ObservableCollection<ComDebugViewModel>();
        AddComDebugViewCommand = new RelayCommand(AddComDebugView, CanAddComDebugView);
        RemoveComDebugViewCommand = new RelayCommand<ComDebugViewModel>(RemoveComDebugView);
        SwitchLanguageCommand = new RelayCommand<string>(SwitchLanguage);
        _timer = new Timer(100);
        _timer.Elapsed += Timer_Elapsed;
    }

    public RelayCommand<string> SwitchLanguageCommand { get; }

    private void SwitchLanguage(string? lan)
    {
        I18nManager.Instance.Culture = CultureInfo.GetCultureInfo(lan);
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        WeakReferenceMessenger.Default.Register<MainViewModel, string, string>(this, nameof(PortUsed), PortUsed);
        WeakReferenceMessenger.Default.Register<MainViewModel, string, string>(this, nameof(PortFree), PortFree);
        _timer.Start();
    }

    private void RemoveComDebugView(ComDebugViewModel? viewModel)
    {
        if (viewModel != null)
        {
            if (viewModel.DisconnectCommand.CanExecute(null)) viewModel.DisconnectCommand.Execute(null);
            ComDebugViewModels.Remove(viewModel);
        }
    }

    private void PortFree(MainViewModel recipient, string message)
    {
        UsedPorts.Remove(message);
        foreach (var viewModel in ComDebugViewModels)
        {
            var ports = Ports.Except(UsedPorts).ToList();
            if (!string.IsNullOrWhiteSpace(viewModel.PortName) && viewModel.Connected) ports.Add(viewModel.PortName);
            viewModel.SetFreePorts(ports.ToArray());
        }
    }

    private void PortUsed(MainViewModel recipient, string message)
    {
        UsedPorts.Add(message);
        foreach (var viewModel in ComDebugViewModels)
        {
            var ports = Ports.Except(UsedPorts).ToList();
            if (!string.IsNullOrWhiteSpace(viewModel.PortName) && viewModel.Connected) ports.Add(viewModel.PortName);
            viewModel.SetFreePorts(ports.ToArray());
        }
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

    protected override void OnViewUnloaded()
    {
        _timer.Stop();
        base.OnViewUnloaded();
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