using ArtCat.Helpers;
using ArtCat.Models;
using ArtCat.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System.Threading;

namespace ArtCat.ViewModels
{
    public class PieceViewModel : Observable
    {
        private Piece _piece;
        private string _name;
        private string _medium;
        private double? _height;
        private double? _width;
        private DateTimeOffset? _dateCompleted;
        private DateTimeOffset? _dateOfferedForSale;
        private DateTimeOffset? _dateSold;
        private double? _priceOffered;
        private double? _priceSold;
        private ICommand _getStorageItemsCommand;
        private ICommand _getPickedFilesCommand;
        private ICommand _deleteImageCommand;
        private ThreadPoolTimer _saveTimer;

        public ICommand GetStorageItemsCommand
            => _getStorageItemsCommand ?? (_getStorageItemsCommand = new RelayCommand<IReadOnlyList<IStorageItem>>(OnGetStorageItems));

        public ICommand GetPickedFilesCommand
            => _getPickedFilesCommand ?? (_getPickedFilesCommand = new RelayCommand(GetFilePickedFiles));

        public ICommand DeleteImageCommand
            => _deleteImageCommand ?? (_deleteImageCommand = new RelayCommand<object>(DeleteImage));

        public string Name
        {
            get
            {
                return _piece.Name;
            }
            set
            {
                if (_piece.Name != value)
                {
                    _piece.Name = value;
                    Set(ref (_name), value);
                    StartSaveTimer();
                }
                
            }

        }

        public double? Height
        {
            get
            {
                return _piece.Height;
            }
            set
            {
                if (_piece.Height != value)
                {
                    _piece.Height = value;
                    Set(ref _height, value);
                    OnPropertyChanged(nameof(HeightIn));
                    OnPropertyChanged(nameof(DisplayDimensions));
                    StartSaveTimer();
                }
            }
        }

        public double? Width
        {
            get
            {
                return _piece.Width;
            }
            set
            {
                if (_piece.Width != value)
                {
                    _piece.Width = value;
                    Set(ref _width, value);
                    OnPropertyChanged(nameof(WidthIn));
                    OnPropertyChanged(nameof(DisplayDimensions));
                    StartSaveTimer();
                }
            }
        }

        public string Medium
        {
            get
            {
                return _piece.Medium;
            }
            set
            {
                if (_piece.Medium != value)
                {
                    _piece.Medium = value;
                    Set(ref _medium, value);
                    StartSaveTimer();
                }
            }
        }

        public string DisplayDimensions
        {
            get
            {
                string result = "";
                if (Width != null && Height != null)
                {
                    result = string.Format("{0}w x {1}h cm", Width, Height);
                }
                return result;
            }
        }

        public DateTimeOffset? DateCompleted
        {
            get
            {
                return _piece.DateCompleted;
            }
            set
            {
                if (_piece.DateCompleted != value)
                {
                    _piece.DateCompleted = value;
                    Set(ref (_dateCompleted), value);
                    StartSaveTimer();
                }
            }
        }

        public DateTimeOffset? DateOfferedForSale
        {
            get
            {
                return _piece.DateOfferedForSale;
            }
            set
            {
                if (_piece.DateOfferedForSale != value)
                {
                    _piece.DateOfferedForSale = value;
                    Set(ref (_dateOfferedForSale), value);
                    StartSaveTimer();
                }
            }
        }

        public DateTimeOffset? DateSold
        {
            get
            {
                return _piece.DateSold;
            }
            set
            {
                if (_piece.DateSold != value)
                {
                    _piece.DateSold = value;
                    Set(ref (_dateSold), value);
                    StartSaveTimer();
                }

            }
        }
        
        public double? PriceOffered
        {
            get
            {
                return _piece.PriceOffered;
            }
            set
            {
                if (_piece.PriceOffered != value)
                {
                    _piece.PriceOffered = value;
                    Set(ref (_priceOffered), value);
                    StartSaveTimer();
                }
            }
        }

        public double? PriceSold
        {
            get
            {
                return _piece.PriceSold;
            }
            set
            {
                if (_piece.PriceSold != value)
                {
                    _piece.PriceSold = value;
                    Set(ref (_priceSold), value);
                    StartSaveTimer();
                }
            }
        }

        public Piece ComponentPiece
        {
            get
            {
                return _piece;
            }
        }

        public ObservableCollection<string> ImagePaths
        {
            get;
        }

        public bool IsImagesEmpty
        {
            get
            {
                return ImagePaths.Count == 0;
            }
        }

        public string WidthIn
        {
            get
            {
                string result = "";
                if (_piece.Width != null)
                {
                    result = string.Format("({0:#.##} in)", _piece.Width * 0.3937);
                }
                return result;
            }
        }

        public string HeightIn
        {
            get
            {
                string result = "";
                if (_piece.Height != null)
                {
                    result = string.Format("({0:#.##} in)", _piece.Height * 0.3937);
                }
                return result;
            }
        }

        public string DisplaySoldInfo
        {
            get
            {
                string result = "";
                if (DateSold != null && PriceSold != null)
                {
                    result = string.Format("Sold on {0:D} for {1:C}", DateSold, PriceSold); 
                }
                return result;
            }
        }


        public string ThumbnailPath
        {
            get
            {
                string result = null;
                if (ImagePaths.Count > 0)
                {
                    result = ImagePaths.ElementAt(0);
                }
                return result;
            }
        }

        public ObservableCollection<string> MediaSuggestions { get; private set; } = new ObservableCollection<string>();

        public PieceViewModel(Piece piece)
        {
            _piece = piece;
            _name = piece.Name;
            ImagePaths = new ObservableCollection<string>();
            if (_piece.Images != null)
            {
                foreach (int index in _piece.Images)
                {
                    ImagePaths.Add(DataService.BaseImagePath + GetNameFromIndex(index));
                } 
            }
            ImagePaths.CollectionChanged += ImagePathsCollectionChanged;
        }

        private void ImagePathsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsImagesEmpty));
            OnPropertyChanged(nameof(ThumbnailPath));
            StartSaveTimer();
        }

        public async void OnGetStorageItems(IReadOnlyList<IStorageItem> items)
        {
            if (items.Count > 0)
            {
                await AddImages(items);
            }
        }
        
        public async void GetFilePickedFiles()
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");

            var list = await picker.PickMultipleFilesAsync();
            if (list.Count > 0)
            {
                await AddImages(list);
            }
        }

        public void DeleteImages()
        {
            List<string> Images = new List<string>(ImagePaths);
            foreach (var image in Images)
            {
                DeleteImage(image);
            }
        }

        public void UpdateMediaSuggestions()
        {
            MediaSuggestions.Clear();
            var suggestions = from medium in DataService.GetMedia()
                              where medium.Contains(Medium, StringComparison.InvariantCultureIgnoreCase)
                              orderby medium
                              select medium;
            foreach (var medium in suggestions)
            {
                MediaSuggestions.Add(medium);
            }
        }

        private async Task AddImages(IEnumerable<IStorageItem> files)
        {
            int index = 0;
            if (_piece.Images.Count > 0)
            {
                index = _piece.Images.Max() + 1;
            }
            else
            {
                index = 1;
            }

            foreach (var item in files)
            {
                if (DataService.IsImageFile(item.Name))
                {
                    string name = _piece.Id + "_" + index;
                    await DataService.SaveImageFile(item, name);
                    if (_piece.Images == null)
                    {
                        _piece.InitialiseNames();
                    }
                    _piece.Images.Add(index);
                    ImagePaths.Add(DataService.BaseImagePath + name);
                    index++; 
                }
            }
        }

        private void DeleteImage(object anImage)
        {
            if (anImage != null)
            {
                string selectedImage = anImage as string;
                ImagePaths.Remove(selectedImage);
                _piece.Images.Remove(GetIndexFromName(DataService.GetFileNameFromPath(selectedImage)));
                //we don't delete the file here, because it might still be locked by the bitmap image
            }
        }

        
        private int GetIndexFromName(string aName)
        {
            int result = 0;
            string fileNumber = aName.Substring(aName.LastIndexOf("_") + 1);
            int i = 0;
            bool found = int.TryParse(fileNumber, out i);
            if (found)
            {
                result = i;
            }
            return result;
        }

        private string GetNameFromIndex(int anIndex)
        {
            return _piece.Id + "_" + anIndex;
        }

        private void StartSaveTimer()
        {
            if (_saveTimer != null)
            {
                _saveTimer.Cancel();
            }
            _saveTimer = ThreadPoolTimer.CreateTimer(TimeIsUp, TimeSpan.FromMilliseconds(2000));
        }

        private void TimeIsUp(ThreadPoolTimer timer)
        {
            DataService.PersistPiece(ComponentPiece);
            Debug.WriteLine("PieceViewModel.cs: The piece was autosaved.");
        }
    }
}
