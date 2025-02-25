namespace Alkitab.Converters;

using System;
using System.Globalization;
using Avalonia.Data.Converters;

public class InverseBooleanConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool booleanValue)
            return !booleanValue; // Invert the boolean value

        return null; // Ensure compatibility with nullability
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    )
    {
        throw new NotImplementedException(); // Not needed for one-way binding
    }
}
