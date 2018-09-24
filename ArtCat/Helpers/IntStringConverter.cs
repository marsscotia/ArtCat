using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ArtCat.Helpers
{
    class IntStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string result = null;
            if (value != null)
            {
                result = value.ToString();
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            double? result = null;
            if (value != null)
            {
                double d;
                bool isNumeric = double.TryParse(value.ToString(), out d);
                if (isNumeric)
                {
                    result = d;
                }
            }
            return result;
        }
    }
}
