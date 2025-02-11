using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using ComAssistant.Lang;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ComAssistant;

internal class I18nExtension
{
    public object? Key { get; set; }

    public object? ProvideValue(IServiceProvider serviceProvider)
    {
        var targetr = ((IProvideValueTarget)serviceProvider).TargetObject;
        if (Key != null) return new I18nBinding(Key);
        return null;
    }
}

internal class I18nBinding : Binding
{
    public I18nBinding(object key)
    {
        Source = I18nManager.Instance;
        Path = nameof(I18nManager.Culture);
        Converter = CultureConverter.Instance;
        ConverterParameter = key;
    }
}

internal class CultureConverter : IValueConverter
{
    private CultureConverter()
    {
    }

    public static IValueConverter Instance { get; } = new CultureConverter();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is string name) return Resources.ResourceManager.GetString(name, value as CultureInfo);
        throw new NotImplementedException();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

internal partial class I18nManager : ObservableObject
{
    [ObservableProperty] private CultureInfo _culture = Resources.Culture;

    private I18nManager()
    {
    }

    public static I18nManager Instance { get; } = new();

    partial void OnCultureChanged(CultureInfo value)
    {

    }
}