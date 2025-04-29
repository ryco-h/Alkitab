using System;
using System.Globalization;
using Alkitab.Models;
using Avalonia.Data.Converters;

namespace Alkitab.Converters;

public class IsBookmarkEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var count = value as int?;
        var target = parameter as string;
        
        Console.WriteLine(count + " " + target);

        if (count == 0 && target == "watermark")
        {
            return true;
        }
        
        if (count > 0 && target == "content")
        {
            return true;
        }
        
        return false;
    }
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}