using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Alkitab.Converters;

public class IsSelectedMultiple : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var count = value as int?;

        return count == 1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}