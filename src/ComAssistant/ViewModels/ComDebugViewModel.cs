using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace ComAssistant.ViewModels;

internal partial class ComDebugViewModel : ViewModelBase
{
    [ObservableProperty] private int _baudRate = 115200;
    [ObservableProperty] private bool _connected;
    [ObservableProperty] private int _dataBits = 8;
    [ObservableProperty] private List<string> _freePorts = new();
    [ObservableProperty] private string? _message;
    [ObservableProperty] private Parity _parity = Parity.None;
    [ObservableProperty] private string? _portName;
    [ObservableProperty] private Encoding _receiveEncoding = Encoding.UTF8;
    [ObservableProperty] private Encoding _sendEncoding = Encoding.UTF8;
    [ObservableProperty] private SerialPort? _serialPort;
    [ObservableProperty] private StopBits _stopBits = StopBits.One;

    public ComDebugViewModel()
    {
        ConnectCommand = new RelayCommand(Connect, CanConnect);
        DisconnectCommand = new RelayCommand(Disconnect, CanDisconnect);
        ClearHistoryCommand = new RelayCommand(ClearHistory, CanClearHistory);
        SendCommand = new RelayCommand(Send, CanSend);
        SendFileCommand = new RelayCommand(SendFile, CanSendFile);
    }

    public ObservableCollection<HistoryItemViewModel> HistoryItems { get; set; } = new();

    private bool CanSendFile()
    {
        return Connected;
    }

    private void SendFile()
    {
    }

    private bool CanSend()
    {
        return !string.IsNullOrWhiteSpace(Message) && Connected;
    }

    partial void OnMessageChanged(string? value)
    {
        SendCommand.NotifyCanExecuteChanged();
    }

    private void Send()
    {
        var buffer = ReceiveEncoding.GetBytes(Message!);
        SerialPort!.Write(buffer, 0, buffer.Length);
        HistoryItems.Add(new HistoryItemViewModel(buffer, true, ReceiveEncoding));
        ClearHistoryCommand.NotifyCanExecuteChanged();
    }

    private bool CanClearHistory()
    {
        return HistoryItems.Any();
    }

    private void ClearHistory()
    {
        HistoryItems.Clear();
    }

    partial void OnFreePortsChanged(List<string> value)
    {
        if (!Connected) PortName = value?.FirstOrDefault();
    }

    private bool CanDisconnect()
    {
        return Connected;
    }

    private void Disconnect()
    {
        SerialPort!.DataReceived += SerialPort_DataReceived;
        SerialPort!.Close();
        SerialPort!.Dispose();
        SerialPort = null;
        Connected = false;
        WeakReferenceMessenger.Default.Send(PortName!, "PortFree");
        DisconnectCommand.NotifyCanExecuteChanged();
        ConnectCommand.NotifyCanExecuteChanged();
    }

    private bool CanConnect()
    {
        if (string.IsNullOrWhiteSpace(PortName)) return false;
        return !Connected;
    }

    private void Connect()
    {
        SerialPort = new SerialPort(PortName, BaudRate, Parity, DataBits, StopBits);
        SerialPort.Encoding = SendEncoding;
        SerialPort.DataReceived += SerialPort_DataReceived;
        SerialPort.Open();
        Connected = true;
        WeakReferenceMessenger.Default.Send(PortName!, "PortUsed");
        DisconnectCommand.NotifyCanExecuteChanged();
        ConnectCommand.NotifyCanExecuteChanged();
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        var buffer = new byte[SerialPort!.BytesToRead];
        SerialPort.Read(buffer, 0, buffer.Length);
        Dispatcher.UIThread.Post(() =>
        {
            HistoryItems.Add(new HistoryItemViewModel(buffer, false, SendEncoding));
            ClearHistoryCommand.NotifyCanExecuteChanged();
        });
    }

    public void SetFreePorts(string[] freePorts)
    {
        var list = freePorts.ToList();
        if (!string.IsNullOrWhiteSpace(PortName) && !freePorts.Contains(PortName)) list.Add(PortName);
        FreePorts = list;
    }

    #region Commands

    [ObservableProperty] private RelayCommand _clearHistoryCommand;
    [ObservableProperty] private RelayCommand _connectCommand;
    [ObservableProperty] private RelayCommand _disconnectCommand;
    [ObservableProperty] private RelayCommand _sendCommand;
    [ObservableProperty] private RelayCommand _sendFileCommand;

    #endregion
}