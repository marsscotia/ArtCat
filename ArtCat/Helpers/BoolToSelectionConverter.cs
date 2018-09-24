using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace ArtCat.Helpers
{
    class BoolToSelectionConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            SelectionMode mode = SelectionMode.Single;
            if ((bool)value)
            {
                mode = SelectionMode.Multiple;
            }
            return mode;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
