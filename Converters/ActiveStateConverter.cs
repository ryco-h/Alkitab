using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Alkitab.Converters;

public class ActiveStateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var currentTarget = (value as string)?.Trim() ?? "";
        var myIdentity = (parameter as string)?.Trim() ?? "";

        return currentTarget == myIdentity ? Brushes.Gray : Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}