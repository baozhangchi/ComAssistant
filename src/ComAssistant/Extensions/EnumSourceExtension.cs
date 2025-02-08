using System;
using Avalonia.Markup.Xaml;

namespace ComAssistant.Extensions;

public class EnumSourceExtension : MarkupExtension
{
    private Type? _enumType;

    public Type? EnumType
    {
        get => _enumType ?? throw new InvalidOperationException();
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(EnumType));

            var enumType = Nullable.GetUnderlyingType(value) ?? value;
            if (!enumType.IsEnum) throw new ArgumentException(nameof(EnumType));

            _enumType = value;
        }
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (EnumType == null) throw new InvalidOperationException("EnumType must be set.");

        var enumType = Nullable.GetUnderlyingType(EnumType) ?? EnumType;
        var values = Enum.GetValues(enumType);
        if (enumType == EnumType) return values;
        var temp = Array.CreateInstance(EnumType, values.Length + 1);
        values.CopyTo(temp, 1);
        return temp;
    }
}