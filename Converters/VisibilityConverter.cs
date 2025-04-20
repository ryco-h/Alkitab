using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Alkitab.Converters;

public class VisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var bookName = value as string;
        Console.WriteLine("BookName: " + bookName);
        return !string.IsNullOrEmpty(bookName);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}