using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace GuideHoundEPG.UI.Converters {
    public class BooleanToVisibilityConverter : IValueConverter {

        public bool InvertVisibility { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {

            bool visible = false;

            if (value != null) {
                visible = (value is bool) && (bool)value;
            }

            if (InvertVisibility) {
                return visible ? Visibility.Collapsed : Visibility.Visible;
            } else {
                return visible ? Visibility.Visible : Visibility.Collapsed;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
