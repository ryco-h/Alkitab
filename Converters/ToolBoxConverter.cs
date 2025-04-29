using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Alkitab.Models;
using Alkitab.Objects;
using Alkitab.Services;
using Alkitab.ViewModels;
using Avalonia.Data.Converters;

namespace Alkitab.Converters;

public class ToolBoxConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isVisible = value is bool b && b;
        
        return isVisible ? 80.0 : 0.0;
    }
    
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}