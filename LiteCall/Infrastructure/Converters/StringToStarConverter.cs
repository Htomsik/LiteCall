using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LiteCall.Infrastructure.Converters
{
    internal  class StringToStarConverter:IValueConverter
    {
        public static readonly IValueConverter Instance = new StringToStarConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            int StarCount = value.ToString().ToArray().Length;

            string StarsSting = "";

            return StarsSting.PadLeft (StarCount,'*');
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
