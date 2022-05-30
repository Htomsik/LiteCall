﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LiteCall.Infrastructure.Converters
{
    internal class IsHighterConverter : IValueConverter
    {
        public static readonly IValueConverter Instance = new IsHighterConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return value.ToString().ToArray().Length > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}