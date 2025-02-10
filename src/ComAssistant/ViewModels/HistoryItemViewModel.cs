using System;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ComAssistant.ViewModels;

internal class HistoryItemViewModel() : ViewModelBase
{
    public HistoryItemViewModel(byte[] message, bool isSend, Encoding encoding) : this()
    {
        Message = message;
        IsSend = isSend;
        Encoding = encoding;
    }

    public HistoryItemViewModel(byte[] message, bool isSend, Encoding encoding, MessageType messageType, string? fileName) : this(message,
        isSend, encoding)
    {
        MessageType = messageType;
        FileName = fileName;
    }

    public bool IsSend { get; set; }
    public bool IsReceived => !IsSend;
    public Encoding Encoding { get; } = Encoding.UTF8;

    public byte[] Message { get; set; } = [];

    public DateTime Time { get; set; } = DateTime.Now;

    public string? FileName { get; set; }

    public MessageType MessageType { get; set; } = MessageType.Text;

    public object? MessageContent => MessageType == MessageType.Text ? Encoding.GetString(Message) : FileName;
}

internal enum MessageType
{
    Text,
    File
}