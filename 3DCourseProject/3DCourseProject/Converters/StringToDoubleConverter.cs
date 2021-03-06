﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace _3DCourseProject.Converters
{
    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.Parse(value.ToString().Replace(".",","));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.Parse(value.ToString().Replace(".", ","));
        }
    }
}
