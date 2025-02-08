using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace ComAssistant.ViewModels;

internal partial class ComDebugViewModel : ViewModelBase
{
    [ObservableProperty] private int _baudRate = 115200;
    [ObservableProperty] private RelayCommand _connectCommand;
    [ObservableProperty] private int _dataBits = 8;
    [ObservableProperty] private RelayCommand _disconnectCommand;
    [ObservableProperty] private Encoding _encoding = Encoding.UTF8;

    [ObservableProperty] private ObservableCollection<Encoding> _encodings = new();
    [ObservableProperty] private string[] _freePorts = [];
    [ObservableProperty] private Parity _parity = Parity.None;
    [ObservableProperty] private string? _portName;
    [ObservableProperty] private string[] _ports = [];
    [ObservableProperty] private SerialPort? _serialPort;
    [ObservableProperty] private StopBits _stopBits = StopBits.One;
    [ObservableProperty] private List<string> _usedPorts = new();

    public ComDebugViewModel()
    {
        WeakReferenceMessenger.Default.Register<ComDebugViewModel, string, string>(this, nameof(PortUsed), PortUsed);
        WeakReferenceMessenger.Default.Register<ComDebugViewModel, string, string>(this, nameof(PortFree), PortFree);
        WeakReferenceMessenger.Default.Register<ComDebugViewModel, string[], string>(this, nameof(PortsRefresh),
            PortsRefresh);
        ConnectCommand = new RelayCommand(Connect, CanConnect);
        DisconnectCommand = new RelayCommand(Disconnect, CanDisconnect);
        Encodings = new ObservableCollection<Encoding>(Encoding.GetEncodings().Select(x => x.GetEncoding()));
    }

    public static int[] BaudRates { get; } = { 2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };
    public static int[] DataBitsArray { get; } = { 8, 9 };

    private void PortFree(ComDebugViewModel recipient, string message)
    {
        if (recipient == this) return;

        UsedPorts.Remove(message);
        FreePorts = Ports.Except(UsedPorts).ToArray();
    }

    partial void OnFreePortsChanged(string[] value)
    {
        PortName = value?.FirstOrDefault();
    }

    partial void OnUsedPortsChanged(List<string> value)
    {
        FreePorts = Ports.Except(UsedPorts).ToArray();
    }

    partial void OnPortsChanged(string[] value)
    {
        FreePorts = Ports.Except(UsedPorts).ToArray();
    }

    private bool CanDisconnect()
    {
        return SerialPort != null && SerialPort.IsOpen;
    }

    private void Disconnect()
    {
        SerialPort!.DataReceived += SerialPort_DataReceived;
        SerialPort!.Close();
        SerialPort!.Dispose();
        SerialPort = null;
        WeakReferenceMessenger.Default.Send(PortName!, nameof(PortFree));
        DisconnectCommand.NotifyCanExecuteChanged();
        ConnectCommand.NotifyCanExecuteChanged();
    }

    private bool CanConnect()
    {
        if (SerialPort != null) return false;

        if (string.IsNullOrWhiteSpace(PortName)) return false;

        if (FreePorts.Length == 0) return false;

        return true;
    }

    private void Connect()
    {
        SerialPort = new SerialPort(PortName, BaudRate, Parity, DataBits, StopBits);
        SerialPort.Encoding = Encoding;
        SerialPort.DataReceived += SerialPort_DataReceived;
        SerialPort.Open();
        WeakReferenceMessenger.Default.Send(PortName!, nameof(PortUsed));
        DisconnectCommand.NotifyCanExecuteChanged();
        ConnectCommand.NotifyCanExecuteChanged();
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var buffer = new byte[SerialPort!.BytesToRead];
        SerialPort.Read(buffer, 0, buffer.Length);
        var message = Encoding.UTF8.GetString(buffer);
        //Application.Current.Dispatcher.BeginInvoke(() =>
        //{
        //    HistoryItems.Add(new HistoryItem { Message = message, Source = true });
        //    NotifyOfPropertyChange(nameof(CanClearHistory));
        //});
    }

    private void PortsRefresh(ComDebugViewModel recipient, string[] message)
    {
        if (recipient == this) return;
        if (Ports.Except(message).Any())
        {
            Ports = message;
            FreePorts = Ports.Except(UsedPorts).ToArray();
        }
    }

    private void PortUsed(ComDebugViewModel recipient, string message)
    {
        if (recipient == this) return;
        UsedPorts.Add(message);
        FreePorts = Ports.Except(UsedPorts).ToArray();
    }
}