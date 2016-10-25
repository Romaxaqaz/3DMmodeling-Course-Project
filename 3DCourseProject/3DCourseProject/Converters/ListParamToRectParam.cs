using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using _3DModeling.Figure;

namespace _3DCourseProject.Converters
{
    public class ListParamToRectParam : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var output = new RectangleGeometry();
            var paramsRectangle = value as HoleRectangle;

            if (paramsRectangle != null)
                output.Rect = new Rect(paramsRectangle.X, paramsRectangle.Y, paramsRectangle.Width, paramsRectangle.Heigth);
            return output.Rect;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
