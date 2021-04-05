using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace b7.Packets.WPF.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility), ParameterType = typeof(bool))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not bool collapse)
                collapse = false;

            return ((bool)value) ? Visibility.Visible : (collapse ? Visibility.Collapsed : Visibility.Hidden);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Visibility)value) == Visibility.Visible;
        }
    }
}
