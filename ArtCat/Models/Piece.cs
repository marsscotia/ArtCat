using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtCat.Models
{
    /// <summary>
    ///
    /// Class represents a piece of art which can be sold.
    /// 
    /// </summary>
    public class Piece
    {
        #region fields

        private Guid _id;
        private string _name;
        private string _medium;
        private double? _height;
        private double? _width;
        private List<int> _images;
        private DateTimeOffset? _dateCompleted;
        private DateTimeOffset? _dateOfferedForSale;
        private DateTimeOffset? _dateSold;
        private double? _priceOffered;
        private double? _priceSold;

        #endregion fields

        #region properties

        //The id of the piece
        public Guid Id { get => _id; set => _id = value; }
        //The name of the piece
        public string Name { get => _name; set => _name = value; }
        //The medium in which the piece was created
        public string Medium { get => _medium; set => _medium = value; }
        //The width of the piece in cm
        public double? Width { get => _width; set => _width = value; }
        //The height of the piece in cm
        public double? Height { get => _height; set => _height = value; }
        //The list of suffix numbers of the stored images relating to this piece
        public List<int> Images { get => _images; set => _images = value; }
        //The date the piece was completed
        public DateTimeOffset? DateCompleted { get => _dateCompleted; set => _dateCompleted = value; }
        //The date the piece was initially offered for sale
        public DateTimeOffset? DateOfferedForSale { get => _dateOfferedForSale; set => _dateOfferedForSale = value; }
        //The date the piece was sold
        public DateTimeOffset? DateSold { get => _dateSold; set => _dateSold = value; }
        //The price the piece was initially offered at
        public double? PriceOffered { get => _priceOffered; set => _priceOffered = value; }
        //The price for which the piece was sold
        public double? PriceSold { get => _priceSold; set => _priceSold = value; }

        #endregion properties

        #region constructors

        public Piece()
        {
            _id = Guid.NewGuid();
            _name = "";
            _images = new List<int>();
        }

        public void InitialiseNames()
        {
            _images = new List<int>();
        }

        #endregion constructors

    }
}
