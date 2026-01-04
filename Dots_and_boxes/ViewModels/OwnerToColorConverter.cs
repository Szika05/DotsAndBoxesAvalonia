using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Dots_and_boxes.ViewModels
{
    /// <summary>
    /// Converts player id's to colors.
    /// </summary>
    public class OwnerToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (value as int?) switch
            {
                1 => Brushes.Blue,
                2 => Brushes.Red,
                _ => Brushes.Transparent
            };
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}