using System;

using ArtCat.ViewModels;

using Windows.UI.Xaml.Controls;

namespace ArtCat.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }
    }
}
