using ArtCat.Models;
using MarcelloDB.Collections;
using MarcelloDB.Index;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtCat.Services
{

    /// <summary>
    /// Class defines the indexed values on the Pieces Collection in
    /// the MarcelloDB. Indexed values are used to provide optimised
    /// searching and iteration.
    /// </summary>
    public class PieceIndexDefinition: IndexDefinition<Piece>
    {
        public IndexedValue<Piece, string> Name
        {
            get
            {
                return IndexedValue(
                    piece => piece.Name,
                    piece => piece.Name != null
                    );
            }

        }
        
    }
}
