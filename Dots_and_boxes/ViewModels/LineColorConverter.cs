using System;
using System.Globalization;
using Avalonia.Media;
using Avalonia.Data.Converters;

namespace Dots_and_boxes.ViewModels
{

    /// <summary>
    /// Change the lines color to black when drawn, else return light gray(when hovered).
    /// </summary>
    public class LineColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isDrawn && isDrawn)
            {
                return Brushes.Black;
            }
            // Return this color when hovered but not drawn
            return Brushes.LightGray;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}