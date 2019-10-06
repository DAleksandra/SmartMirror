using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;


namespace SmartMirrorRpPi.Converters
{
    public class StringFormatConverter : IValueConverter

    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var format = parameter as string;
            if (string.IsNullOrEmpty(format))
            {
                return value;
            }

            return string.Format(format, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
