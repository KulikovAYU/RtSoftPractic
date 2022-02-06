using Avalonia.Media;
using ClientApp.Models;
using System;
using System.Globalization;

namespace ClientView.Infrastructure.ValueConverters
{
    public class FilterOperationValueConverter : BaseValueConverter<FilterOperationValueConverter>
    {
        public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is State stat)
            { 
                if (stat == State.Started)
                    return new SolidColorBrush(Color.FromRgb(0, 255, 0));

                if (stat == State.Stopped)
                    return new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            return null;
        }

        public override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
