namespace Alkitab.Converters;

using System;
using System.Globalization;
using Avalonia.Data.Converters;
using System.Collections.Generic;

public class MultiParamConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        var bookName = values[0]?.ToString();
        var pasal = values[1]?.ToString();
        var direction = values[2]?.ToString();

        return new { BookName = bookName, Pasal = pasal, Direction = direction };
    }
}