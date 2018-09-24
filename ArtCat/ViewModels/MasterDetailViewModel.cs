using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ArtCat.Helpers;
using ArtCat.Models;
using ArtCat.Services;

using Microsoft.Toolkit.Uwp.UI.Controls;

namespace ArtCat.ViewModels
{
    public class MasterDetailViewModel : Observable
    {
        private PieceViewModel _selected;
        private RelayCommand<object> _deletePieceCommand;
        private string _searchTerm;

        public PieceViewModel Selected
        {
            get { return _selected; }
            set
            {
                PersistPiece();
                Set(ref _selected, value);
            }
        }
        
        public ObservableCollection<PieceViewModel> Pieces { get; private set; } = new ObservableCollection<PieceViewModel>();

        public RelayCommand<object> DeletePieceCommand
            => _deletePieceCommand ?? (_deletePieceCommand = new RelayCommand<object>(DeletePiece));

        public ObservableCollection<string> SearchSuggestions { get; private set; } = new ObservableCollection<string>();

        public string SearchTerm
        {
            get
            {
                return _searchTerm;
            }
            set
            {
                Set(ref _searchTerm, value);
                if (string.IsNullOrEmpty(SearchTerm))
                {
                    ClearFilter();
                }
            }
        }

        public MasterDetailViewModel()
        {
        }

        public void LoadData(MasterDetailsViewState viewState)
        {
            ClearFilter();

            if (viewState == MasterDetailsViewState.Both)
            {
                if (Pieces.Count > 0)
                {
                    Selected = Pieces.First(); 
                }
                else
                {
                    Selected = null;
                }
            }
        }

        public void AddPiece()
        {
            Piece newPiece = new Piece();
            PieceViewModel newPieceViewModel = new PieceViewModel(newPiece);
            Pieces.Add(newPieceViewModel);
            Selected = newPieceViewModel;
        }

        public void PersistPiece()
        {
            if (Selected != null)
            {
                DataService.PersistPiece(Selected.ComponentPiece);
            }
        }

        public void UpdateSearchSuggestions()
        {
            SearchSuggestions.Clear();
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                var suggestions = from piece in DataService.GetNames()
                                  where piece.Contains(SearchTerm, StringComparison.InvariantCultureIgnoreCase)
                                  orderby piece
                                  select piece;
                foreach (var name in suggestions)
                {
                    SearchSuggestions.Add(name);
                }
            }
        }

        private void DeletePiece(object aPiece)
        {
            if (aPiece != null)
            {
                PieceViewModel selectedPieceViewModel = aPiece as PieceViewModel;
                Piece selectedPiece = selectedPieceViewModel.ComponentPiece;
                selectedPieceViewModel.DeleteImages();
                Pieces.Remove(selectedPieceViewModel);
                DataService.DestroyPiece(selectedPiece);

            }
        }
        
        public void FilterPieces()
        {
            Pieces.Clear();
            var pieces = DataService.FindPieces(SearchTerm);
            foreach (var piece in pieces)
            {
                Pieces.Add(new PieceViewModel(piece));
            }
        }

        public void ClearFilter()
        {
            Pieces.Clear();
            var data = DataService.GetPieces();
            foreach (var piece in data)
            {
                Pieces.Add(new PieceViewModel(piece));
            }
        }
    }
}
