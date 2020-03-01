using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QM
{
    /// <summary>
    /// Converter subtracts ConverterParametr's value from binded value. 
    /// </summary>
    public class SubtractionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse((string)parameter, out int p) && value is double v)
            {
                return v - p;
            }

            if (int.TryParse((string)parameter, out int pp) && value is int vv)
            {
                return vv - pp;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter returns binded value divided by ConverterParametr's value. 
    /// </summary>
    public class DivisionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse((string)parameter, out int p) && value is double v)
            {
                return v / p;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
