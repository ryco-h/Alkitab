using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Alkitab.Converters;

public class IndentationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string verse)
            return int.Parse(verse) % 2 == 0 ? new Thickness(40, 5, 5, 5) : new Thickness(5);

        return new Thickness(5); // default fallback
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}