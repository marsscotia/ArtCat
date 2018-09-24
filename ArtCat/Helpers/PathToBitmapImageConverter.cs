using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace ArtCat.Helpers
{
    /// <summary>
    /// Class converts a uri path to a BitmapImage.  This is required as the
    /// UriSource property on 
    /// </summary>
    public class PathToBitmapImageConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            BitmapImage result = new BitmapImage();
            string path = value as string;
            if (!string.IsNullOrWhiteSpace(path))
            {
                result.UriSource = new Uri(path);
                result.DecodePixelWidth = 56;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
