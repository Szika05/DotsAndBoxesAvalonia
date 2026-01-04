using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Dots_and_boxes.ViewModels
{
    /// <summary>
    /// Calculates the positions of the dots.
    /// </summary>
    public class MultiplierConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int v && double.TryParse(parameter?.ToString(), out double factor))
                return v * factor;
            return 0.0;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}