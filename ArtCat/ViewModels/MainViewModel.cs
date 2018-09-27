using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ArtCat.Helpers;
using ArtCat.Models;
using ArtCat.Services;

namespace ArtCat.ViewModels
{
    public class MainViewModel : Observable
    {
        private DateTimeOffset? _startDate;
        private DateTimeOffset? _endDate;
        //this flag can be set to prevent the data from being calculated
        private bool _preventCalc = false;

        public DateTimeOffset? StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                Set(ref _startDate, value);
                if (!_preventCalc)
                {
                    PopulateData();
                }
            }
        }

        public DateTimeOffset? EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                Set(ref _endDate, value);
                if (!_preventCalc)
                {
                    PopulateData();
                }
            }
        }

        public ObservableCollection<PieceViewModel> Pieces { get; private set; } = new ObservableCollection<PieceViewModel>();

        public bool IsPiecesEmpty => Pieces.Count == 0;
        public bool IsPiecesNotEmpty => !IsPiecesEmpty;

        public string NumberPiecesSold => Pieces.Count.ToString();

        public string MinPrice
        {
            get
            {
                string result = "";
                if (IsPiecesNotEmpty)
                {
                    double? min = Pieces.Min(piece => piece.PriceSold);
                    if (min != null)
                    {
                        result = string.Format("{0:C}", min);
                    }
                }
                return result;
            }
        }

        public string MaxPrice
        {
            get
            {
                string result = "";
                if (IsPiecesNotEmpty)
                {
                    double? max = Pieces.Max(piece => piece.PriceSold);
                    if (max != null)
                    {
                        result = string.Format("{0:C}", max);
                    }
                }
                return result;
            }
        }

        public string AvgPrice
        {
            get
            {
                string result = "";
                if (IsPiecesNotEmpty)
                {
                    double? avg = Pieces.Average(piece => piece.PriceSold);
                    if (avg != null)
                    {
                        result = string.Format("{0:C}", avg);
                    }
                }
                return result;
            }
        }

        public string TotalPrice
        {
            get
            {
                string result = "";
                if (IsPiecesNotEmpty)
                {
                    double? sum = Pieces.Sum(piece => piece.PriceSold);
                    if (sum != null)
                    {
                        result = string.Format("{0:C}", sum);
                    }
                }
                return result;
            }
        }


        public MainViewModel()
        {
            Pieces.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(IsPiecesEmpty));
                OnPropertyChanged(nameof(IsPiecesNotEmpty));
            };
        }

        public void LoadData()
        {
            _preventCalc = true;
            List<Piece> pieces = DataService.GetPieces();
            StartDate = pieces.Min(piece => piece.DateSold);
            EndDate = pieces.Max(piece => piece.DateSold);
            PopulateData();
            _preventCalc = false;
        }

        private void PopulateData()
        {
            Pieces.Clear();
            if (StartDate != null && EndDate != null)
            {
                List<Piece> pieces = DataService.FindSoldPieces(StartDate, EndDate);
                foreach (var piece in pieces)
                {
                    Pieces.Add(new PieceViewModel(piece));
                }
            }
            if (IsPiecesNotEmpty)
            {
                OnPropertyChanged(nameof(NumberPiecesSold));
                OnPropertyChanged(nameof(MinPrice));
                OnPropertyChanged(nameof(MaxPrice));
                OnPropertyChanged(nameof(AvgPrice));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }
    }
}
