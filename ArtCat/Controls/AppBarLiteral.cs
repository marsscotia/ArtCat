using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace ArtCat.Controls
{
    /// <summary>
    ///
    /// Control for adding content to an app bar,
    /// in place of the app bar separator.
    ///
    /// Uses code authored by Jerry Nixon: https://gist.github.com/JerryNixon/c797538f9c3e4e5c4c1039354020c05f
    /// 
    /// </summary>
    [ContentProperty(Name =nameof(LiteralContent))]
    public sealed class AppBarLiteral: AppBarSeparator
    {
        public static readonly DependencyProperty LiteralContentProperty
            = DependencyProperty.Register(nameof(LiteralContent), typeof(object), typeof(AppBarLiteral), new PropertyMetadata(null));

        public AppBarLiteral()
        {
            DefaultStyleKey = typeof(AppBarLiteral);
        }
        
        public object LiteralContent
        {
            get
            {
                return (object)GetValue(LiteralContentProperty);
            }
            set
            {
                SetValue(LiteralContentProperty, value);
            }
            
        }
    }
}
