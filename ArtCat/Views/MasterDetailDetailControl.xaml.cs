using System;

using ArtCat.Models;
using ArtCat.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ArtCat.Views
{
    public sealed partial class MasterDetailDetailControl : UserControl
    {
        public PieceViewModel MasterMenuItem
        {
            get { return GetValue(MasterMenuItemProperty) as PieceViewModel; }
            set { SetValue(MasterMenuItemProperty, value); }
        }

        public static readonly DependencyProperty MasterMenuItemProperty = DependencyProperty.Register("MasterMenuItem", typeof(PieceViewModel), typeof(MasterDetailDetailControl), new PropertyMetadata(null, OnMasterMenuItemPropertyChanged));

        public MasterDetailDetailControl()
        {
            InitializeComponent();
        }

        private static void OnMasterMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MasterDetailDetailControl;
            control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}
