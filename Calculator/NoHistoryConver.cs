using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Calculator
{
    class NoHistoryConver:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)//源到目标
        {
            if ((bool)value)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)//目标到源
        {
            throw new NotImplementedException();
        }
    }
}
