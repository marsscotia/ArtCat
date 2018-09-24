using System;

using ArtCat.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ArtCat.Views
{
    public sealed partial class MasterDetailPage : Page
    {
        public MasterDetailViewModel ViewModel { get; } = new MasterDetailViewModel();

        public MasterDetailPage()
        {
            InitializeComponent();
            Loaded += MasterDetailPage_Loaded;
            DataContext = ViewModel;
        }

        private void MasterDetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadData(MasterDetailsViewControl.ViewState);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            ViewModel.PersistPiece();
        }
    }
}
