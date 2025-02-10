using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Layout;

namespace ComAssistant.Converters;

internal class HorizontalAlignmentConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b && !b) return HorizontalAlignment.Right;
        return HorizontalAlignment.Left;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}