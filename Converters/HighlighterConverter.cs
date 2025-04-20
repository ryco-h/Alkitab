using System;
using System.Globalization;
using Alkitab.Services;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Alkitab.Converters;

public class HighlighterConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string verse)
        {
            var selectedVerse = ToggleService.Instance.ToggleState.Ayat;
            return int.Parse(verse) == int.Parse(selectedVerse) ? Brushes.LightBlue : Brushes.Transparent;
        }

        return Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}