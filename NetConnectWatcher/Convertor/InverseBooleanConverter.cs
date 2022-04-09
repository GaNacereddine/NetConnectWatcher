using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NetConnectWatcher.Convertor
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject())) return true;
#endif

            if ((value == null) || (!(value is bool boolValue)))
            {
                throw new ArgumentException("value must be of type bool, value is required");
            }

            return !boolValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
